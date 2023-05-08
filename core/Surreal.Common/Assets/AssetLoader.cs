namespace Surreal.Assets;

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

  protected abstract Task<T> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken);
}
