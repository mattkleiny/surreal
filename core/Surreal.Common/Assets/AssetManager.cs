using System.Diagnostics.CodeAnalysis;
using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Base class for settings for a particular type of asset.</summary>
public abstract record AssetSettings<T>;

/// <summary>Represents uniquely some asset type at a given path.</summary>
public readonly record struct AssetId(Type Type, VirtualPath Path)
{
  public override string ToString() => Path.ToString();
}

/// <summary>Allows managing assets.</summary>
public interface IAssetManager : IDisposable
{
  bool IsHotReloadEnabled { get; }

  void AddLoader(IAssetLoader loader);

  bool TryGetSettings<TAsset>(VirtualPath path, [NotNullWhen(true)] out AssetSettings<TAsset>? settings);
  void SetSettings<TAsset>(VirtualPath path, AssetSettings<TAsset> settings);

  ValueTask<TAsset> LoadAsset<TAsset>(VirtualPath path, CancellationToken cancellationToken = default);

  IDisposable SubscribeToChanges(AssetId id, VirtualPath path, AssetChangedHandler<object> handler);
}

/// <summary>The default <see cref="IAssetManager"/> implementation.</summary>
public sealed class AssetManager : IAssetManager
{
  private readonly List<IAssetLoader> loaders = new();
  private readonly Dictionary<AssetId, object> assetsById = new();
  private readonly Dictionary<AssetId, object> settingsById = new();

  public bool IsHotReloadEnabled { get; set; } = true;

  public void AddLoader(IAssetLoader loader)
  {
    loaders.Add(loader);
  }

  public bool TryGetSettings<T>(VirtualPath path, [NotNullWhen(true)] out AssetSettings<T>? result)
  {
    var id = new AssetId(typeof(T), path);

    if (settingsById.TryGetValue(id, out var settings))
    {
      result = (AssetSettings<T>) settings;
      return true;
    }

    result = default;
    return false;
  }

  public void SetSettings<T>(VirtualPath path, AssetSettings<T> settings)
  {
    var id = new AssetId(typeof(T), path);

    settingsById[id] = settings;
  }

  public async ValueTask<TAsset> LoadAsset<TAsset>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    var id = new AssetId(typeof(TAsset), path);
    var context = new AssetLoaderContext(id, this);

    if (!TryGetLoader(context, out var loader))
    {
      throw new UnsupportedAssetException($"An unsupported asset type was requested: {context.Type.Name}");
    }

    if (!assetsById.TryGetValue(id, out var asset))
    {
      // we'll continue asynchronously on the main thread
      assetsById[id] = asset = await loader.LoadAsync(context, cancellationToken);
    }

    return (TAsset) asset;
  }

  public IDisposable SubscribeToChanges(AssetId id, VirtualPath path, AssetChangedHandler<object> handler)
  {
    // not all file systems support listening for changes
    if (!path.SupportsWatching())
    {
      return Disposables.Null;
    }

    // we'll serialize top-level change notifications down to listeners
    var modificationLock = new object();

    // dispatches path change events to particular handlers.
    async void OnPathModified(VirtualPath changedPath)
    {
      if (changedPath != path)
      {
        return; // there's a bit of multiplexing going on here
      }

      Monitor.Enter(modificationLock);
      try
      {
        if (assetsById.TryGetValue(id, out var asset))
        {
          var context = new AssetLoaderContext(id, this);

          assetsById[id] = await handler(context, asset, CancellationToken.None);
        }
      }
      catch (Exception)
      {
        // TODO: handle me? or ignore me?
      }
      finally
      {
        Monitor.Exit(modificationLock);
      }
    }

    // TODO: wire this up inside an 'asset' container, allowing it to manage internal state
    // TODO: free watchers and listeners when the associated asset goes away

    var watcher = path.Watch();

    watcher.Created  += OnPathModified;
    watcher.Modified += OnPathModified;
    watcher.Deleted  += OnPathModified;

    return Disposables.Anonymous(() =>
    {
      watcher.Created  -= OnPathModified;
      watcher.Modified -= OnPathModified;
      watcher.Deleted  -= OnPathModified;

      watcher.Dispose();
    });
  }

  public void Dispose()
  {
    foreach (var asset in assetsById.Values)
    {
      if (asset is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    foreach (var loader in loaders)
    {
      if (loader is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    assetsById.Clear();
    loaders.Clear();
  }

  /// <summary>Attempts to locate a valid loader for the given type.</summary>
  private bool TryGetLoader(AssetLoaderContext context, [NotNullWhen(true)] out IAssetLoader? result)
  {
    for (var i = 0; i < loaders.Count; i++)
    {
      var loader = loaders[i];
      if (loader.CanHandle(context))
      {
        result = loader;
        return true;
      }
    }

    result = default;
    return false;
  }
}

/// <summary>Denotes the given asset type is not supported by the manager.</summary>
public sealed class UnsupportedAssetException : Exception
{
  public UnsupportedAssetException(string message)
    : base(message)
  {
  }
}
