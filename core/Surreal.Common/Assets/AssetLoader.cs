using Surreal.IO;

namespace Surreal.Assets;

/// <summary>A callback for when assets change, allowing transparent reload of asset contents.</summary>
public delegate ValueTask<T> AssetChangedHandler<T>(AssetLoaderContext context, T existingAsset, CancellationToken cancellationToken)
  where T : notnull;

/// <summary>Context for <see cref="IAssetLoader"/> operations.</summary>
public readonly record struct AssetLoaderContext(AssetId Id, IAssetManager Manager)
{
  public Type        AssetType => Id.Type;
  public VirtualPath Path      => Id.Path;

  public bool IsHotReloadEnabled => Manager.IsHotReloadEnabled;

  /// <summary>Listens for changes in the associated asset.</summary>
  public IDisposable RegisterForChanges<T>(AssetChangedHandler<T> handler)
    where T : notnull => RegisterForChanges(Path, handler);

  /// <summary>Listens for changes in the associated asset, sub-delineated by the given path.</summary>
  public IDisposable RegisterForChanges<T>(VirtualPath path, AssetChangedHandler<T> handler)
    where T : notnull => Manager.RegisterForChanges(Id, path, async (context, existingAsset, cancellationToken) =>
  {
    return await handler(context, (T) existingAsset, cancellationToken);
  });

  /// <summary>Loads a dependent asset from the associated manager.</summary>
  public ValueTask<T> LoadDependencyAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    // TODO: wire up the dependency graph in here
    return Manager.LoadAsset<T>(path, cancellationToken);
  }

  /// <summary>Loads a dependent asset from the associated manager.</summary>
  public ValueTask<T> LoadDependencyAsync<T>(VirtualPath path, Optional<object> parameters, CancellationToken cancellationToken = default)
  {
    return Manager.LoadAsset<T>(path, parameters, cancellationToken);
  }
}

/// <summary>Allows loading assets from storage.</summary>
public interface IAssetLoader
{
  bool CanHandle(AssetLoaderContext context);

  ValueTask<object> LoadAsync(AssetLoaderContext context, Optional<object> parameters, CancellationToken cancellationToken);
}

/// <summary>Base class for any <see cref="IAssetLoader"/> implementation.</summary>
public abstract class AssetLoader<TAsset> : IAssetLoader
  where TAsset : notnull
{
  public virtual bool CanHandle(AssetLoaderContext context)
  {
    return context.AssetType == typeof(TAsset);
  }

  public abstract ValueTask<TAsset> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken);

  async ValueTask<object> IAssetLoader.LoadAsync(AssetLoaderContext context, Optional<object> parameters, CancellationToken cancellationToken)
  {
    return await LoadAsync(context, cancellationToken);
  }
}

/// <summary>Base class for any <see cref="IAssetLoader"/> implementation that accepts parameters.</summary>
public abstract class AssetLoader<TAsset, TParameters> : IAssetLoader
  where TAsset : notnull
  where TParameters : class, new()
{
  public TParameters DefaultParameters { get; set; } = new();

  public virtual bool CanHandle(AssetLoaderContext context)
  {
    return context.AssetType == typeof(TAsset);
  }

  public abstract ValueTask<TAsset> LoadAsync(AssetLoaderContext context, TParameters parameters, CancellationToken cancellationToken);

  async ValueTask<object> IAssetLoader.LoadAsync(AssetLoaderContext context, Optional<object> parameters, CancellationToken cancellationToken)
  {
    return await LoadAsync(context, (TParameters) parameters.GetOrDefault(DefaultParameters), cancellationToken);
  }
}
