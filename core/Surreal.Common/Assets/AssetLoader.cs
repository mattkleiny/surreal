using Surreal.IO;

namespace Surreal.Assets;

/// <summary>
/// Allows loading assets from storage.
/// </summary>
public interface IAssetLoader
{
  /// <summary>
  /// Determines if the loader can handle the given asset.
  /// </summary>
  bool CanHandle(AssetContext context);

  /// <summary>
  /// Loads an asset from storage.
  /// </summary>
  Task<object> LoadAsync(AssetContext context, CancellationToken cancellationToken);
}

/// <summary>
/// Base class for any <see cref="IAssetLoader" /> implementation.
/// </summary>
public abstract class AssetLoader<T> : IAssetLoader
  where T : notnull
{
  public virtual bool CanHandle(AssetContext context)
  {
    return context.Type == typeof(T);
  }

  public abstract Task<T> LoadAsync(AssetContext context, CancellationToken cancellationToken);

  async Task<object> IAssetLoader.LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    return await LoadAsync(context, cancellationToken);
  }
}

/// <summary>
/// Context for <see cref="IAssetLoader" /> operations.
/// </summary>
public readonly record struct AssetContext(AssetId AssetId, AssetManager Manager)
{
  /// <summary>
  /// The type of the asset being loaded.
  /// </summary>
  public Type Type => AssetId.Type;

  /// <summary>
  /// The path of the asset being loaded.
  /// </summary>
  public VirtualPath Path => AssetId.Path;

  /// <summary>
  /// Loads a dependent asset from the manager.
  /// </summary>
  public Task<T> LoadAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    return Manager.LoadAssetAsync<T>(path, cancellationToken);
  }

  /// <summary>
  /// Watches for changes on the given path and notifies the given <see cref="IHotReloadable{T}"/> when a change occurs.
  /// </summary>
  public void ReloadWhenChanged<T>(IHotReloadable<T> reloadable)
  {
    if (Manager.IsHotReloadEnabled)
    {
      Manager.WatchForChanges(AssetId, reloadable);
    }
  }
}
