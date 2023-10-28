using Surreal.Collections;
using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Editing.Projects;

/// <summary>
/// An asset database is a collection of files and folders that are used to create a game.
/// </summary>
public sealed class AssetDatabase(string sourcePath, string targetPath)
{
  private readonly AssetEntryCollection _entries = new();

  /// <summary>
  /// Source path where assets are loaded.
  /// </summary>
  public string SourcePath { get; } = Path.GetFullPath(sourcePath);

  /// <summary>
  /// The target path where cooked assets are saved.
  /// </summary>
  public string TargetPath { get; } = Path.GetFullPath(targetPath);

  /// <summary>
  /// A list of <see cref="IAssetImporter"/>s that are capable of loading raw assets from disk.
  /// </summary>
  public List<IAssetImporter> Importers { get; } = new();

  /// <summary>
  /// All of the assets in the database.
  /// </summary>
  public IEnumerable<AssetEntry> Assets => _entries;

  /// <summary>
  /// Gets all assets in the database at the given path.
  /// </summary>
  public AssetEntry GetAssetsById(Guid id)
  {
    if (!_entries.TryGetById(id, out var entry))
    {
      throw new InvalidOperationException($"No asset with ID {id} exists in the database");
    }

    return entry;
  }

  /// <summary>
  /// Gets all assets in the database at the given path.
  /// </summary>
  public IEnumerable<AssetEntry> GetAssetsByPath(string path)
  {
    var absolutePath = Path.GetFullPath(path);

    if (!_entries.TryGetByPath(absolutePath, out var entries))
    {
      return Enumerable.Empty<AssetEntry>();
    }

    return entries;
  }

  /// <summary>
  /// Gets all assets in the database at the given path.
  /// </summary>
  public IEnumerable<AssetEntry> GetAssetsByType(Type assetType)
  {
    if (!assetType.TryGetCustomAttribute(out AssetTypeAttribute attribute))
    {
      throw new InvalidOperationException("The given type has no associated asset type attribute");
    }

    if (!_entries.TryGetByType(attribute.Id, out var entries))
    {
      return Enumerable.Empty<AssetEntry>();
    }

    return entries;
  }

  /// <summary>
  /// Refreshes the asset database.
  /// </summary>
  public async Task RefreshAsync(CancellationToken cancellationToken = default)
  {
    _entries.Clear();

    // recursively search for all files in the source path
    foreach (var assetPath in Directory.GetFiles(SourcePath, "*", SearchOption.AllDirectories))
    {
      VirtualPath metadataPath = Path.ChangeExtension(assetPath, "meta");

      if (assetPath.EndsWith(".meta")) continue; // we don't want to import metadata directly
      if (!await metadataPath.ExistsAsync()) continue; // we don't want to import assets without metadata

      var metadata = await metadataPath.DeserializeAsync<AssetMetadata>(FileFormat.Yml, cancellationToken);

      foreach (var importer in Importers)
      {
        if (importer.CanHandle(metadata))
        {
          _entries.Add(new AssetEntry
          {
            AssetId = metadata.AssetId,
            TypeId = metadata.TypeId,
            AbsolutePath = assetPath,
            IsEmbedded = false // TODO: detect embedded assets?
          });

          break; // TODO: support multiple assets per path
        }
      }
    }
  }

  /// <summary>
  /// Bakes the asset database for the given target.
  /// </summary>
  public Task BakeAsync(string target, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// A single entry in the <see cref="AssetDatabase"/>.
  /// </summary>
  [DebuggerDisplay("{AssetId} at {AbsolutePath}")]
  public sealed class AssetEntry
  {
    /// <summary>
    /// The ID of the asset.
    /// </summary>
    public required Guid AssetId { get; init; }

    /// <summary>
    /// The type ID of the asset.
    /// </summary>
    public required Guid TypeId { get; init; }

    /// <summary>
    /// The absolute path to the asset.
    /// </summary>
    public required string AbsolutePath { get; init; }

    /// <summary>
    /// True if this asset is embedded in the output assembly.
    /// </summary>
    public required bool IsEmbedded { get; init; }

    /// <summary>
    /// The current value of the asset in memory.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// The index of this entry in the <see cref="AssetEntryCollection"/>.
    /// </summary>
    internal ArenaIndex Index { get; set; }
  }

  /// <summary>
  /// A managed collection of <see cref="AssetEntry"/>.
  /// </summary>
  private sealed class AssetEntryCollection : IEnumerable<AssetEntry>
  {
    private readonly Arena<AssetEntry> _entries = new();
    private readonly Dictionary<Guid, ArenaIndex> _assetsById = new();
    private readonly MultiDictionary<string, ArenaIndex> _assetsByPath = new();
    private readonly MultiDictionary<Guid, ArenaIndex> _assetsByType = new();

    /// <summary>
    /// Attempts to get an asset by its ID.
    /// </summary>
    public bool TryGetById(Guid key, [MaybeNullWhen(false)] out AssetEntry result)
    {
      if (!_assetsById.TryGetValue(key, out var index))
      {
        result = default;
        return false;
      }

      result = _entries[index];
      return true;
    }

    /// <summary>
    /// Attempts to get all assets by path.
    /// </summary>
    public bool TryGetByPath(string path, [MaybeNullWhen(false)] out AssetEntry[] results)
    {
      if (!_assetsByPath.TryGetValues(path, out var indices))
      {
        results = default;
        return false;
      }

      results = new AssetEntry[indices.Length];

      for (var i = 0; i < indices.Length; i++)
      {
        results[i] = _entries[indices[i]];
      }

      return true;
    }

    /// <summary>
    /// Attempts to get all assets by type.
    /// </summary>
    public bool TryGetByType(Guid key, [MaybeNullWhen(false)] out AssetEntry[] results)
    {
      if (!_assetsByType.TryGetValues(key, out var indices))
      {
        results = default;
        return false;
      }

      results = new AssetEntry[indices.Length];

      for (var i = 0; i < indices.Length; i++)
      {
        results[i] = _entries[indices[i]];
      }

      return true;
    }

    /// <summary>
    /// Adds a new asset to the collection.
    /// </summary>
    public void Add(AssetEntry entry)
    {
      var index = _entries.Add(entry);

      entry.Index = index;

      _assetsById.Add(entry.AssetId, index);
      _assetsByPath.Add(entry.AbsolutePath, index);
      _assetsByType.Add(entry.TypeId, index);
    }

    /// <summary>
    /// Removes an existing asset from the collection.
    /// </summary>
    public void Remove(AssetEntry entry)
    {
      var index = entry.Index;

      _entries.Remove(index);

      _assetsById.Remove(entry.AssetId);
      _assetsByPath.Remove(entry.AbsolutePath, index);
      _assetsByType.Remove(entry.TypeId, index);
    }

    /// <summary>
    /// Clears the collection.
    /// </summary>
    public void Clear()
    {
      _entries.Clear();

      _assetsById.Clear();
      _assetsByType.Clear();
      _assetsByPath.Clear();

      _entries.Compact();
    }

    public Arena<AssetEntry>.Enumerator GetEnumerator()
    {
      return _entries.GetEnumerator();
    }

    IEnumerator<AssetEntry> IEnumerable<AssetEntry>.GetEnumerator()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
