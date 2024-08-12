using Surreal.Assets;
using Surreal.Collections;
using Surreal.Diagnostics.Logging;
using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Editing.Assets;

/// <summary>
/// An asset database is a collection of files and folders that are used to create a game.
/// </summary>
public sealed class AssetDatabase(string sourcePath, string targetPath) : IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<AssetDatabase>();

  private readonly AssetEntryCollection _entries = [];
  private IPathWatcher? _pathWatcher;

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
  public List<IAssetImporter> Importers { get; init; } = [];

  /// <summary>
  /// All of the assets in the database.
  /// </summary>
  public IEnumerable<AssetEntry> Assets => _entries;

  /// <summary>
  /// Watches for changes on the source path and automatically imports them.
  /// </summary>
  public bool WatchForChanges
  {
    get => _pathWatcher != null;
    set
    {
      if (value)
      {
        VirtualPath basePath = SourcePath;

        _pathWatcher = basePath.Watch(includeSubPaths: true);

        _pathWatcher.Created += OnFileCreated;
        _pathWatcher.Changed += OnFileChanged;
        _pathWatcher.Renamed += OnFileRenamed;
        _pathWatcher.Deleted += OnFileDeleted;
      }
      else if (_pathWatcher != null)
      {
        _pathWatcher.Created -= OnFileCreated;
        _pathWatcher.Changed -= OnFileChanged;
        _pathWatcher.Renamed -= OnFileRenamed;
        _pathWatcher.Deleted -= OnFileDeleted;

        _pathWatcher.Dispose();
        _pathWatcher = null;
      }
    }
  }

  /// <summary>
  /// True if the database should automatically import changes when <see cref="WatchForChanges"/> is true.
  /// </summary>
  public bool AutomaticallyImportChanges { get; set; }

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
      return [];
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
      throw new InvalidOperationException($"The associated class {assetType.Name} has no {nameof(AssetTypeAttribute)}");
    }

    if (!_entries.TryGetByType(attribute.Id, out var entries))
    {
      return [];
    }

    return entries;
  }

  /// <summary>
  /// Imports all assets under the given path into the database.
  /// </summary>
  public async Task ImportAssetsAsync(CancellationToken cancellationToken = default, bool writeMetadataToDisk = false)
  {
    Log.Trace($"Importing asset database from {SourcePath}");

    // recursively search for all files in the source path
    foreach (var absolutePath in Directory.GetFiles(SourcePath, "*", SearchOption.AllDirectories))
    {
      VirtualPath assetPath = absolutePath;
      VirtualPath metadataPath = Path.ChangeExtension(absolutePath, "meta");

      try
      {
        if (_entries.ContainsPath(absolutePath)) continue; // we don't want to import assets that are already imported
        if (absolutePath.EndsWith(".meta")) continue; // we don't want to import metadata directly

        if (metadataPath.Exists())
        {
          // refresh this entry
          var metadata = await metadataPath.DeserializeAsync<AssetMetadata>(FileFormat.Yml, cancellationToken);

          Log.Trace($"Importing asset from path {assetPath}");

          _entries.Add(new AssetEntry
          {
            AbsolutePath = absolutePath,
            AssetId = metadata.AssetId,
            TypeId = metadata.TypeId,
            IsEmbedded = false // TODO: detect embedded assets?
          });
        }
        else
        {
          // import anew
          foreach (var importer in Importers)
          {
            if (importer.TryGetTypeId(absolutePath, out var typeId))
            {
              Log.Trace($"Importing new asset from path {assetPath}");

              _entries.Add(new AssetEntry
              {
                AbsolutePath = absolutePath,
                AssetId = Guid.NewGuid(),
                TypeId = typeId,
                IsEmbedded = false // TODO: detect embedded assets?
              });

              if (writeMetadataToDisk)
              {
                var metadata = new AssetMetadata
                {
                  AssetId = Guid.NewGuid(),
                  TypeId = typeId
                };

                Log.Trace($"Writing asset metadata to disk {metadataPath}");

                await metadataPath.SerializeAsync(metadata, FileFormat.Yml, cancellationToken);
              }
            }
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, $"An error occurred whilst importing asset {assetPath}");
      }
    }
  }

  /// <summary>
  /// Refreshes the asset database.
  /// </summary>
  public async Task RefreshAssetsAsync(CancellationToken cancellationToken = default)
  {
    Log.Trace($"Refreshing asset database from {SourcePath}");

    _entries.Clear();

    // recursively search for all files in the source path
    foreach (var absolutePath in Directory.GetFiles(SourcePath, "*", SearchOption.AllDirectories))
    {
      VirtualPath assetPath = absolutePath;
      VirtualPath metadataPath = Path.ChangeExtension(absolutePath, "meta");

      try
      {
        if (absolutePath.EndsWith(".meta")) continue; // we don't want to import metadata directly
        if (!metadataPath.Exists()) continue; // we don't want to import assets without metadata

        var metadata = await metadataPath.DeserializeAsync<AssetMetadata>(FileFormat.Yml, cancellationToken);

        foreach (var importer in Importers)
        {
          if (importer.CanHandle(metadata))
          {
            Log.Trace($"Refreshing asset {metadata.AssetId} from {assetPath}");

            _entries.Add(new AssetEntry
            {
              AssetId = metadata.AssetId,
              TypeId = metadata.TypeId,
              AbsolutePath = absolutePath,
              IsEmbedded = false // TODO: detect embedded assets?
            });

            break; // TODO: support multiple assets per path
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, $"An error occurred whilst refreshing asset {assetPath}");
      }
    }
  }

  /// <summary>
  /// Bakes the asset database for the given target.
  /// </summary>
  public async Task BakeTargetAsync(IAssetBaker baker, AssetBakingTarget target, CancellationToken cancellationToken = default)
  {
    var assets = _entries.Where(x => !x.IsEmbedded).ToArray();
    var outputPath = Path.Combine(TargetPath, target.Path);

    await baker.BakeAssetsAsync(assets, target, outputPath, cancellationToken);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    _pathWatcher?.Dispose();
  }

  /// <summary>
  /// Invoked when a file is created.
  /// </summary>
  private void OnFileCreated(VirtualPath path)
  {
    if (path.IsFile())
    {
      Log.Trace($"File was created {path}");

      if (AutomaticallyImportChanges)
      {
        // TODO: implement me
      }
    }
    else if (path.IsDirectory())
    {
      Log.Trace($"Directory was created {path}");
    }
  }

  /// <summary>
  /// Invoked when a file is modified.
  /// </summary>
  private void OnFileChanged(VirtualPath path, PathChangeTypes changeType)
  {
    if (changeType.HasFlagFast(PathChangeTypes.Modified))
    {
      if (path.IsFile())
      {
        Log.Trace($"File was modified {path}");

        // TODO: automatically reload the asset in the database?
      }
      else if (path.IsDirectory())
      {
        Log.Trace($"Directory was modified {path}");
      }
    }
  }

  /// <summary>
  /// Invoked when a file is renamed.
  /// </summary>
  private void OnFileRenamed(VirtualPath oldPath, VirtualPath newPath)
  {
    if (oldPath.IsFile())
    {
      Log.Trace($"File was renamed from {oldPath} to {newPath}");

      // TODO: automatically rename the asset in the database and the manifest on disk?
    }
    else if (oldPath.IsDirectory())
    {
      Log.Trace($"Directory was renamed from {oldPath} to {newPath}");
    }
  }

  /// <summary>
  /// Invoked when a file is deleted.
  /// </summary>
  private void OnFileDeleted(VirtualPath path)
  {
    if (path.IsFile())
    {
      Log.Trace($"File was deleted {path}");

      if (_entries.TryGetByPath(path.Target.ToString(), out var entries))
      {
        _entries.RemoveAll(entries);
      }
    }
    else if (path.IsDirectory())
    {
      Log.Trace($"Directory was deleted {path}");
    }
  }

  /// <summary>
  /// A single entry in the <see cref="AssetDatabase"/>.
  /// </summary>
  [DebuggerDisplay("{AssetId} at {AbsolutePath}")]
  public sealed class AssetEntry : IBakeableAsset
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
    /// The index of this entry in the <see cref="AssetEntryCollection"/>.
    /// </summary>
    internal ArenaIndex Index { get; set; }
  }

  /// <summary>
  /// A managed collection of <see cref="AssetEntry"/>.
  /// </summary>
  private sealed class AssetEntryCollection : IEnumerable<AssetEntry>
  {
    private readonly Arena<AssetEntry> _entries = [];
    private readonly Dictionary<Guid, ArenaIndex> _assetsById = new();
    private readonly MultiDictionary<string, ArenaIndex> _assetsByPath = new();
    private readonly MultiDictionary<Guid, ArenaIndex> _assetsByTypeId = new();

    /// <summary>
    /// Determines if the collection contains an asset with the given ID.
    /// </summary>
    public bool ContainsId(Guid assetId)
    {
      return _assetsById.ContainsKey(assetId);
    }

    /// <summary>
    /// Attempts to get an asset by its ID.
    /// </summary>
    public bool TryGetById(Guid assetId, [MaybeNullWhen(false)] out AssetEntry result)
    {
      if (!_assetsById.TryGetValue(assetId, out var index))
      {
        result = default;
        return false;
      }

      result = _entries[index];
      return true;
    }

    /// <summary>
    /// Determines if the collection contains an asset at the given path.
    /// </summary>
    public bool ContainsPath(string absolutePath)
    {
      return _assetsByPath.ContainsKey(absolutePath);
    }

    /// <summary>
    /// Attempts to get all assets by path.
    /// </summary>
    public bool TryGetByPath(string absolutePath, [MaybeNullWhen(false)] out AssetEntry[] results)
    {
      if (!_assetsByPath.TryGetValues(absolutePath, out var indices))
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
    /// Determines if the collection contains an asset with the given type id.
    /// </summary>
    public bool ContainsType(Guid typeId)
    {
      return _assetsByTypeId.ContainsKey(typeId);
    }

    /// <summary>
    /// Attempts to get all assets by type id.
    /// </summary>
    public bool TryGetByType(Guid typeId, [MaybeNullWhen(false)] out AssetEntry[] results)
    {
      if (!_assetsByTypeId.TryGetValues(typeId, out var indices))
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
      _assetsByTypeId.Add(entry.TypeId, index);
    }

    /// <summary>
    /// Adds all of the given entries.
    /// </summary>
    public void AddAll(IEnumerable<AssetEntry> entries)
    {
      foreach (var entry in entries)
      {
        Add(entry);
      }
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
      _assetsByTypeId.Remove(entry.TypeId, index);
    }

    /// <summary>
    /// Removes all of the given entries.
    /// </summary>
    public void RemoveAll(IEnumerable<AssetEntry> entries)
    {
      foreach (var entry in entries)
      {
        Remove(entry);
      }
    }

    /// <summary>
    /// Clears the collection.
    /// </summary>
    public void Clear()
    {
      _entries.Clear();

      _assetsById.Clear();
      _assetsByTypeId.Clear();
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
