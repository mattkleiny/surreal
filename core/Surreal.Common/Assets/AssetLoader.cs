using Surreal.IO;

namespace Surreal.Assets;

/// <summary>A callback for when assets change, allowing transparent reload of asset contents.</summary>
public delegate Task<T> AssetChangedHandler<T>(AssetLoaderContext context, T existingAsset, CancellationToken cancellationToken)
  where T : notnull;

/// <summary>Context for <see cref="IAssetLoader" /> operations.</summary>
public readonly record struct AssetLoaderContext(AssetId Id, IAssetManager Manager)
{
  public Type Type => Id.Type;
  public VirtualPath Path => Id.Path;

  public bool IsHotReloadEnabled => Manager.IsHotReloadEnabled;

  /// <summary>Listens for changes in the associated asset.</summary>
  public IDisposable SubscribeToChanges<T>(AssetChangedHandler<T> handler)
    where T : notnull
  {
    return Manager.SubscribeToChanges(Id, Path, async (context, existingAsset, cancellationToken) =>
    {
      return await handler(context, (T) existingAsset, cancellationToken);
    });
  }

  /// <summary>Loads a dependent asset from the associated manager.</summary>
  public Task<T> LoadAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    return Manager.LoadAssetAsync<T>(path, cancellationToken);
  }
}

/// <summary>Allows loading assets from storage.</summary>
public interface IAssetLoader
{
  bool CanHandle(AssetLoaderContext context);

  Task<object> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken);
}

/// <summary>Base class for any <see cref="IAssetLoader" /> implementation.</summary>
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

/// <summary>Base class for any <see cref="IAssetLoader" /> implementation.</summary>
public abstract class AssetLoader<T, TSettings> : IAssetLoader
  where T : notnull
  where TSettings : AssetSettings<T>, new()
{
  public TSettings Settings { get; set; } = new();

  public virtual bool CanHandle(AssetLoaderContext context)
  {
    return context.Type == typeof(T);
  }

  async Task<object> IAssetLoader.LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var settings = GetAssetParameters(context);

    return await LoadAsync(context, settings, cancellationToken);
  }

  /// <summary>Gets the <see cref="TSettings" /> for the given asset context.</summary>
  public TSettings GetAssetParameters(AssetLoaderContext context)
  {
    if (context.Manager.TryGetSettings<T>(context.Path, out var settings))
    {
      return (TSettings) settings;
    }

    return Settings;
  }

  public abstract Task<T> LoadAsync(AssetLoaderContext context, TSettings settings, CancellationToken cancellationToken);
}

