using Surreal.IO;

namespace Surreal.Assets;

/// <summary>
/// A callback for when assets change, allowing transparent reload of asset contents.
/// </summary>
public delegate Task<T> AssetChangedHandler<T>(AssetLoaderContext context, T existingAsset, CancellationToken cancellationToken)
  where T : notnull;

/// <summary>
/// Context for <see cref="IAssetLoader" /> operations.
/// </summary>
public readonly record struct AssetLoaderContext(AssetId Id, IAssetManager Manager)
{
  /// <summary>
  /// The type of the asset being loaded.
  /// </summary>
  public Type Type => Id.Type;

  /// <summary>
  /// The path of the asset being loaded.
  /// </summary>
  public VirtualPath Path => Id.Path;

  /// <summary>
  /// True if hot reload is enabled on the asset manager.
  /// </summary>
  public bool IsHotReloadEnabled => Manager.IsHotReloadEnabled;

  /// <summary>
  /// Loads a dependent asset from the manager.
  /// </summary>
  public Task<T> LoadAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    return Manager.LoadAssetAsync<T>(path, cancellationToken);
  }

  /// <summary>
  /// Listens for changes in the associated asset.
  /// </summary>
  public IDisposable SubscribeToChanges<T>(AssetChangedHandler<T> handler)
    where T : notnull
  {
    return Manager.SubscribeToChanges(Id, Path, async (context, existingAsset, cancellationToken) =>
    {
      var newAsset = await handler(context, (T)existingAsset, cancellationToken);

      return newAsset;
    });
  }
}
