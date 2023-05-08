using Surreal.IO;

namespace Surreal.Resources;

/// <summary>
/// Allows loading resources from storage.
/// </summary>
public interface IResourceLoader
{
  /// <summary>
  /// Determines if the loader can handle the given asset.
  /// </summary>
  bool CanHandle(ResourceContext context);

  /// <summary>
  /// Loads an asset from storage.
  /// </summary>
  Task<object> LoadAsync(ResourceContext context, CancellationToken cancellationToken);
}

/// <summary>
/// Base class for any <see cref="IResourceLoader" /> implementation.
/// </summary>
public abstract class ResourceLoader<T> : IResourceLoader
  where T : notnull
{
  public virtual bool CanHandle(ResourceContext context)
  {
    return context.Type == typeof(T);
  }

  public abstract Task<T> LoadAsync(ResourceContext context, CancellationToken cancellationToken);

  async Task<object> IResourceLoader.LoadAsync(ResourceContext context, CancellationToken cancellationToken)
  {
    return await LoadAsync(context, cancellationToken);
  }
}

/// <summary>
/// Context for <see cref="IResourceLoader" /> operations.
/// </summary>
public readonly record struct ResourceContext(ResourceId Id, IResourceManager Manager)
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
    return Manager.LoadResourceAsync<T>(path, cancellationToken);
  }

  /// <summary>
  /// Listens for changes in the associated asset.
  /// </summary>
  public IDisposable SubscribeToChanges<T>(ResourceChangeListener<T> handler)
    where T : notnull
  {
    async Task<object> OnResourceChanged(ResourceContext context, object existingAsset, CancellationToken cancellationToken)
    {
      return await handler(context, (T)existingAsset, cancellationToken);
    }

    return Manager.SubscribeToChanges(Id, OnResourceChanged);
  }
}

/// <summary>
/// A callback for when assets change, allowing transparent reload of asset contents.
/// </summary>
public delegate Task<T> ResourceChangeListener<T>(ResourceContext context, T existingAsset, CancellationToken cancellationToken);
