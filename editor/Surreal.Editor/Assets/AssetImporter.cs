using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Assets;

/// <summary>
/// An asset importer is capable of loading raw assets from disk.
/// <para/>
/// Raw assets can then be converted into cooked assets depending upon the pipeline.
/// </summary>
public interface IAssetImporter
{
  /// <summary>
  /// Determines if the given path can be handled by this importer, and returns the expected type id.
  /// </summary>
  bool TryGetTypeId(string absolutePath, out Guid typeId);

  /// <summary>
  /// Determines if the importer can handle an asset.
  /// </summary>
  bool CanHandle(AssetMetadata metadata);

  /// <summary>
  /// Imports a raw asset from disk.
  /// </summary>
  Task<object> ImportAsync(VirtualPath path, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base class for asset importers.
/// </summary>
public abstract class AssetImporter<[MeansImplicitUse] TAsset> : IAssetImporter
  where TAsset : class
{
  /// <summary>
  /// Determines if the importer can handle an asset at the given path.
  /// </summary>
  protected abstract bool CanHandlePath(string path);

  /// <summary>
  /// Imports a raw <see cref="TAsset"/> from disk.
  /// </summary>
  public abstract Task<TAsset> ImportAsync(VirtualPath path, CancellationToken cancellationToken = default);

  /// <inheritdoc/>
  bool IAssetImporter.TryGetTypeId(string path, out Guid typeId)
  {
    if (CanHandlePath(path))
    {
      if (!typeof(TAsset).TryGetCustomAttribute(out AssetTypeAttribute attribute))
      {
        throw new InvalidOperationException($"The associated class {typeof(TAsset)} has no {nameof(AssetTypeAttribute)}");
      }

      typeId = attribute.Id;
      return true;
    }

    typeId = default;
    return false;
  }

  /// <inheritdoc/>
  bool IAssetImporter.CanHandle(AssetMetadata metadata)
  {
    if (!typeof(TAsset).TryGetCustomAttribute(out AssetTypeAttribute attribute))
    {
      throw new InvalidOperationException($"The associated class {typeof(TAsset)} has no {nameof(AssetTypeAttribute)}");
    }

    return attribute.Id == metadata.TypeId;
  }

  /// <inheritdoc/>
  async Task<object> IAssetImporter.ImportAsync(VirtualPath path, CancellationToken cancellationToken)
  {
    return await ImportAsync(path, cancellationToken);
  }
}
