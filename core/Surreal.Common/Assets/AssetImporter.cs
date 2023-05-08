namespace Surreal.Assets;

/// <summary>
/// Imports assets from a file format.
/// </summary>
public interface IAssetImporter
{
  /// <summary>
  /// The type of resource that this importer imports.
  /// </summary>
  Type ResourceType { get; }

  /// <summary>
  /// Determines if this importer can handle the specified path.
  /// </summary>
  bool CanHandle(string path);

  /// <summary>
  /// Imports an asset into a resource.
  /// </summary>
  Task<object> ImportAsync(string path, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base class for <see cref="IAssetImporter"/>s.
/// </summary>
public abstract class AssetImporter<TResource> : IAssetImporter
  where TResource : class
{
  /// <inheritdoc/>
  public Type ResourceType => typeof(TResource);

  /// <inheritdoc/>
  public abstract bool CanHandle(string path);

  /// <summary>
  /// Imports an asset into a <see cref="TResource"/>.
  /// </summary>
  public abstract Task<TResource> ImportAsync(string path, CancellationToken cancellationToken = default);

  async Task<object> IAssetImporter.ImportAsync(string path, CancellationToken cancellationToken)
  {
    return await ImportAsync(path, cancellationToken);
  }
}
