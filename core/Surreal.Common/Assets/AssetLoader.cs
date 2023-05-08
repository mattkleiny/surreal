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

/// <summary>
/// Allows loading assets from storage.
/// </summary>
public interface IAssetLoader
{
  /// <summary>
  /// Determines if the loader can handle the given asset.
  /// </summary>
  bool CanHandle(AssetLoaderContext context);

  /// <summary>
  /// Loads an asset from storage.
  /// </summary>
  Task<object> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken);
}

/// <summary>
/// Base class for any <see cref="IAssetLoader" /> implementation.
/// </summary>
public abstract class AssetLoader<T> : IAssetLoader
  where T : notnull
{
  public virtual bool CanHandle(AssetLoaderContext context)
  {
    return context.Type == typeof(T);
  }

  async Task<object> IAssetLoader.LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    return await LoadAsync(context, cancellationToken);
  }

  public abstract Task<T> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken);
}
