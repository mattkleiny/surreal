using Surreal.IO;

namespace Surreal.Assets;

/// <summary>A callback for when assets change, allowing transparent reload of asset contents.</summary>
public delegate ValueTask<T> AssetChangedHandler<T>(AssetLoaderContext context, T existingAsset, CancellationToken cancellationToken)
  where T : notnull;

/// <summary>Context for <see cref="IAssetLoader"/> operations.</summary>
public readonly record struct AssetLoaderContext(AssetId Id, IAssetManager Manager)
{
  public Type        Type => Id.Type;
  public VirtualPath Path => Id.Path;

  public bool IsHotReloadEnabled => Manager.IsHotReloadEnabled;

  /// <summary>Listens for changes in the associated asset.</summary>
  public IDisposable SubscribeToChanges<T>(AssetChangedHandler<T> handler)
    where T : notnull => SubscribeToChanges(Path, handler);

  /// <summary>Listens for changes in the associated asset, sub-delineated by the given path.</summary>
  public IDisposable SubscribeToChanges<T>(VirtualPath path, AssetChangedHandler<T> handler)
    where T : notnull => Manager.SubscribeToChanges(Id, path, async (context, existingAsset, cancellationToken) =>
  {
    return await handler(context, (T) existingAsset, cancellationToken);
  });

  /// <summary>Loads a dependent asset from the associated manager.</summary>
  public ValueTask<T> LoadAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
    => Manager.LoadAsset<T>(path, cancellationToken);
}

/// <summary>Allows loading assets from storage.</summary>
public interface IAssetLoader
{
  bool CanHandle(AssetLoaderContext context);

  ValueTask<object> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken);
}

/// <summary>Base class for any <see cref="IAssetLoader"/> implementation.</summary>
public abstract class AssetLoader<T> : IAssetLoader
  where T : notnull
{
  public virtual bool CanHandle(AssetLoaderContext context)
  {
    return context.Type == typeof(T);
  }

  public abstract ValueTask<T> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken);

  async ValueTask<object> IAssetLoader.LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    return await LoadAsync(context, cancellationToken);
  }
}

/// <summary>Base class for any <see cref="IAssetLoader"/> implementation.</summary>
public abstract class AssetLoader<T, TSettings> : IAssetLoader
  where T : notnull
  where TSettings : AssetSettings<T>, new()
{
  public TSettings Settings { get; set; } = new();

  /// <summary>Gets the <see cref="TSettings"/> for the given asset context.</summary>
  public TSettings GetAssetParameters(AssetLoaderContext context)
  {
    if (context.Manager.TryGetSettings<T>(context.Path, out var settings))
    {
      return (TSettings) settings;
    }

    return Settings;
  }

  public virtual bool CanHandle(AssetLoaderContext context)
  {
    return context.Type == typeof(T);
  }

  public abstract ValueTask<T> LoadAsync(AssetLoaderContext context, TSettings settings, CancellationToken cancellationToken);

  async ValueTask<object> IAssetLoader.LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var settings = GetAssetParameters(context);

    return await LoadAsync(context, settings, cancellationToken);
  }
}
