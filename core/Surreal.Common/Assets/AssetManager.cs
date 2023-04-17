using Surreal.Diagnostics.Logging;
using Surreal.IO;

namespace Surreal.Assets;

/// <summary>
/// Base class for settings for a particular type of asset.
/// </summary>
public abstract record AssetSettings<T>;

/// <summary>
/// Represents uniquely some asset type at a given path.
/// </summary>
public readonly record struct AssetId(Type Type, VirtualPath Path)
{
  public override string ToString()
  {
    return Path.ToString();
  }
}

/// <summary>
/// Allows managing assets.
/// </summary>
public interface IAssetManager : IDisposable
{
  bool IsHotReloadEnabled { get; }

  void AddLoader(IAssetLoader loader);

  void AddSettings<T>(VirtualPath path, AssetSettings<T> settings);
  bool TryGetSettings<T>(VirtualPath path, [NotNullWhen(true)] out AssetSettings<T>? results);

  bool IsAssetLoaded<T>(VirtualPath path);
  bool TryGetAsset<T>(VirtualPath path, [NotNullWhen(true)] out T? result);
  Task<T> LoadAssetAsync<T>(VirtualPath path, CancellationToken cancellationToken = default);

  IDisposable SubscribeToChanges(AssetId id, VirtualPath path, AssetChangedHandler<object> handler);
}

/// <summary>
/// The default <see cref="IAssetManager" /> implementation.
/// </summary>
public sealed class AssetManager : IAssetManager
{
  private static readonly ILog Log = LogFactory.GetLog<AssetManager>();
  private readonly Dictionary<AssetId, object> _assetsById = new();

  private readonly List<IAssetLoader> _loaders = new();
  private readonly Dictionary<AssetId, object> _settingsById = new();
  private readonly List<IPathWatcher> _watchers = new();

  public bool IsHotReloadEnabled { get; set; } = true;

  public void AddLoader(IAssetLoader loader)
  {
    _loaders.Add(loader);
  }

  public bool TryGetSettings<T>(VirtualPath path, [NotNullWhen(true)] out AssetSettings<T>? results)
  {
    var id = new AssetId(typeof(T), path);

    if (_settingsById.TryGetValue(id, out var settings))
    {
      results = (AssetSettings<T>)settings;
      return true;
    }

    results = default;
    return false;
  }

  public bool IsAssetLoaded<T>(VirtualPath path)
  {
    var id = new AssetId(typeof(T), path);

    return _assetsById.ContainsKey(id);
  }

  public bool TryGetAsset<T>(VirtualPath path, [NotNullWhen(true)] out T? result)
  {
    var id = new AssetId(typeof(T), path);

    if (_assetsById.TryGetValue(id, out var asset))
    {
      result = (T)asset;
      return true;
    }

    result = default;
    return false;
  }

  public void AddSettings<T>(VirtualPath path, AssetSettings<T> settings)
  {
    var id = new AssetId(typeof(T), path);

    _settingsById[id] = settings;
  }

  public async Task<T> LoadAssetAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    var id = new AssetId(typeof(T), path);
    var context = new AssetLoaderContext(id, this);

    if (!TryGetLoader(context, out var loader))
    {
      throw new UnsupportedAssetException($"An unsupported asset type was requested: {context.Type.Name}");
    }

    if (!_assetsById.TryGetValue(id, out var asset))
      // we'll continue asynchronously on the main thread
    {
      _assetsById[id] = asset = await loader.LoadAsync(context, cancellationToken);
    }

    return (T)asset;
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

      Log.Trace($"{changedPath} was modified, notifying subscribers");

      Monitor.Enter(modificationLock);
      try
      {
        if (_assetsById.TryGetValue(id, out var asset))
        {
          var context = new AssetLoaderContext(id, this);

          _assetsById[id] = await handler(context, asset, CancellationToken.None);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, $"An occurred whilst processing a hot loaded asset {id}");
      }
      finally
      {
        Monitor.Exit(modificationLock);
      }
    }

    // TODO: wire this up inside an 'asset' container, allowing it to manage internal state
    // TODO: free watchers and listeners when the associated asset goes away

    var watcher = path.Watch();

    watcher.Created += OnPathModified;
    watcher.Modified += OnPathModified;
    watcher.Deleted += OnPathModified;

    _watchers.Add(watcher);

    return Disposables.Anonymous(() =>
    {
      watcher.Created -= OnPathModified;
      watcher.Modified -= OnPathModified;
      watcher.Deleted -= OnPathModified;

      watcher.Dispose();
    });
  }

  public void Dispose()
  {
    foreach (var asset in _assetsById.Values)
    {
      if (asset is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    foreach (var loader in _loaders)
    {
      if (loader is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    foreach (var watcher in _watchers) watcher.Dispose();

    _assetsById.Clear();
    _loaders.Clear();
    _watchers.Clear();
  }

  /// <summary>
  /// Attempts to locate a valid loader for the given type.
  /// </summary>
  private bool TryGetLoader(AssetLoaderContext context, [NotNullWhen(true)] out IAssetLoader? result)
  {
    for (var i = 0; i < _loaders.Count; i++)
    {
      var loader = _loaders[i];
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

/// <summary>
/// Denotes the given asset type is not supported by the manager.
/// </summary>
public sealed class UnsupportedAssetException : Exception
{
  public UnsupportedAssetException(string message)
    : base(message)
  {
  }
}
