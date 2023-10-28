﻿using Surreal.Assets;
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
      throw new InvalidOperationException($"The associated class {assetType.Name} has no {nameof(AssetTypeAttribute)}");
    }

    if (!_entries.TryGetByType(attribute.Id, out var entries))
    {
      return Enumerable.Empty<AssetEntry>();
    }

    return entries;
  }

  /// <summary>
  /// Loads a <see cref="TAsset"/> from the given path.
  /// </summary>
  public async Task<TAsset> LoadAssetAsync<TAsset>(string path, CancellationToken cancellationToken = default, bool writeMetadataToDisk = false)
    where TAsset : class
  {
    return (TAsset)await LoadAssetAsync(typeof(TAsset), path, cancellationToken, writeMetadataToDisk);
  }

  /// <summary>
  /// Loads an asset of the given type from the given path.
  /// </summary>
  private async Task<object> LoadAssetAsync(Type assetType, string path, CancellationToken cancellationToken = default, bool writeMetadataToDisk = false)
  {
    var absolutePath = Path.GetFullPath(path);

    // if an asset exists at the given path, load it
    foreach (var entry in GetAssetsByType(assetType))
    {
      // we're looking for an asset with the same absolute path
      if (entry.AbsolutePath != absolutePath) continue;

      var metadata = new AssetMetadata
      {
        AssetId = entry.AssetId,
        TypeId = entry.TypeId
      };

      foreach (var importer in Importers)
      {
        if (importer.CanHandle(metadata))
        {
          return await importer.ImportAsync(absolutePath, cancellationToken);
        }
      }
    }

    // load a brand new asset and add it to the database
    return ImportAssetAsync(assetType, path, cancellationToken, writeMetadataToDisk);
  }

  /// <summary>
  /// Imports a new asset into the database.
  /// </summary>
  public async Task<object> ImportAssetAsync(Type assetType, string path, CancellationToken cancellationToken = default, bool writeMetadataToDisk = false)
  {
    if (path.EndsWith(".meta"))
    {
      throw new InvalidOperationException("Unable to import .meta file into the database; they're markers for the database itself");
    }

    var absolutePath = Path.GetFullPath(path);

    // create a new asset metadata
    var metadata = new AssetMetadata
    {
      AssetId = Guid.NewGuid(),
      TypeId = assetType.GetCustomAttribute<AssetTypeAttribute>()!.Id
    };

    foreach (var importer in Importers)
    {
      if (importer.CanHandle(metadata))
      {
        var asset = await importer.ImportAsync(absolutePath, cancellationToken);

        // create a new asset entry
        _entries.Add(new AssetEntry
        {
          AssetId = metadata.AssetId,
          TypeId = metadata.TypeId,
          AbsolutePath = absolutePath,
          IsEmbedded = false // TODO: detect embedded assets?
        });

        if (writeMetadataToDisk)
        {
          // write the metadata to disk
          VirtualPath metadataPath = Path.ChangeExtension(absolutePath, "meta");

          await metadataPath.SerializeAsync(metadata, FileFormat.Yml, cancellationToken);
        }

        return asset;
      }
    }

    throw new InvalidOperationException($"Unable to locate asset importer for {assetType.Name}");
  }

  /// <summary>
  /// Imports all assets under the given path into the database.
  /// </summary>
  public async Task ImportAssetsAsync(string basePath, CancellationToken cancellationToken = default, bool writeMetadataToDisk = false)
  {
    // recursively search for all files in the source path
    foreach (var absolutePath in Directory.GetFiles(SourcePath, "*", SearchOption.AllDirectories))
    {
      if (_entries.ContainsPath(absolutePath)) continue; // we don't want to import assets that are already imported

      VirtualPath metadataPath = Path.ChangeExtension(absolutePath, "meta");

      if (absolutePath.EndsWith(".meta")) continue; // we don't want to import metadata directly
      if (await metadataPath.ExistsAsync()) continue; // we don't want to import assets that already have metadata

      foreach (var importer in Importers)
      {
        if (importer.TryDetermineType(absolutePath, out var typeId))
        {
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

            await metadataPath.SerializeAsync(metadata, FileFormat.Yml, cancellationToken);
          }
        }
      }
    }
  }

  /// <summary>
  /// Refreshes the asset database.
  /// </summary>
  public async Task RefreshAsync(CancellationToken cancellationToken = default)
  {
    _entries.Clear();

    // recursively search for all files in the source path
    foreach (var absolutePath in Directory.GetFiles(SourcePath, "*", SearchOption.AllDirectories))
    {
      VirtualPath metadataPath = Path.ChangeExtension(absolutePath, "meta");

      if (absolutePath.EndsWith(".meta")) continue; // we don't want to import metadata directly
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
            AbsolutePath = absolutePath,
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
