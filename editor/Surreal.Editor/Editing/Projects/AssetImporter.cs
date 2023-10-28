﻿using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Editing.Projects;

/// <summary>
/// An asset importer is capable of loading raw assets from disk.
/// <para/>
/// Raw assets can then be converted into cooked assets depending upon the pipeline.
/// </summary>
public interface IAssetImporter
{
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
  /// <inheritdoc/>
  public virtual bool CanHandle(AssetMetadata metadata)
  {
    if (!typeof(TAsset).TryGetCustomAttribute(out AssetTypeAttribute attribute))
    {
      throw new InvalidOperationException($"The associated class {typeof(TAsset)} has no {nameof(AssetTypeAttribute)}");
    }

    return attribute.Id == metadata.TypeId;
  }

  /// <summary>
  /// Imports a raw <see cref="TAsset"/> from disk.
  /// </summary>
  public abstract Task<TAsset> ImportAsync(VirtualPath path, CancellationToken cancellationToken = default);

  /// <inheritdoc/>
  async Task<object> IAssetImporter.ImportAsync(VirtualPath path, CancellationToken cancellationToken)
  {
    return await ImportAsync(path, cancellationToken);
  }
}

/// <summary>
/// Metadata about how to read and process an asset.
/// </summary>
public sealed record AssetMetadata
{
  /// <summary>
  /// A unique identifier for this asset.
  /// </summary>
  public required Guid AssetId { get; init; }

  /// <summary>
  /// The type of the asset.
  /// </summary>
  public required Guid TypeId { get; init; }
}

/// <summary>
/// Indicates the associated type is an asset.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AssetTypeAttribute(string id) : Attribute
{
  /// <summary>
  /// The unique identifier for this asset type.
  /// </summary>
  public Guid Id { get; } = Guid.Parse(id);
}
