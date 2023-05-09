using Surreal.Diagnostics.Logging;
using Surreal.IO;

namespace Surreal.Resources;

/// <summary>
/// Represents uniquely some resource type at a given path.
/// </summary>
public readonly record struct ResourceId(Type Type, VirtualPath Path)
{
  public override string ToString() => Path.ToString();
}

/// <summary>
/// Allows managing resources.
/// </summary>
public interface IResourceManager : IDisposable
{
  /// <summary>
  /// Determines if hot reload is enabled.
  /// </summary>
  bool IsHotReloadEnabled { get; }

  /// <summary>
  /// Adds a loader to the resource manager.
  /// </summary>
  void AddLoader(IResourceLoader loader);

  /// <summary>
  /// Loads the resource at the given path.
  /// </summary>
  Task<T> LoadResourceAsync<T>(VirtualPath path, CancellationToken cancellationToken = default);

  /// <summary>
  /// Subscribes to changes in the given resource at the given path.
  /// </summary>
  IDisposable SubscribeToChanges(ResourceId id, ResourceChangeListener<object> handler);
}

/// <summary>
/// The default <see cref="IResourceManager" /> implementation.
/// </summary>
public sealed class ResourceManager : IResourceManager
{
  private static readonly ILog Log = LogFactory.GetLog<ResourceManager>();

  private readonly Dictionary<ResourceId, object> _resourcesById = new();
  private readonly List<IResourceLoader> _loaders = new();
  private readonly List<IPathWatcher> _watchers = new();

  public bool IsHotReloadEnabled { get; set; } = true;

  public void AddLoader(IResourceLoader loader)
  {
    _loaders.Add(loader);
  }

  public async Task<T> LoadResourceAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    var id = new ResourceId(typeof(T), path);
    var context = new ResourceContext(id, this);

    if (!TryGetLoader(context, out var loader))
    {
      throw new UnsupportedResourceException($"An unsupported asset type was requested: {context.Type.Name}");
    }

    if (!_resourcesById.TryGetValue(id, out var asset))
    {
      _resourcesById[id] = asset = await loader.LoadAsync(context, cancellationToken);
    }

    return (T)asset;
  }

  public IDisposable SubscribeToChanges(ResourceId id, ResourceChangeListener<object> handler)
  {
    // not all file systems support listening for changes
    if (!id.Path.SupportsWatching())
    {
      return Disposables.Null;
    }

    // we'll serialize top-level change notifications down to listeners
    var modificationLock = new object();

    // dispatches path change events to particular handlersW
    async void OnPathModified(VirtualPath changedPath)
    {
      if (changedPath != id.Path)
      {
        return; // there's a bit of multiplexing going on here
      }

      Log.Trace($"{changedPath} was modified, notifying subscribers");

      Monitor.Enter(modificationLock);
      try
      {
        if (_resourcesById.TryGetValue(id, out var asset))
        {
          var context = new ResourceContext(id, this);

          _resourcesById[id] = await handler(context, asset, CancellationToken.None);
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

    var watcher = id.Path.Watch();

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
    foreach (var asset in _resourcesById.Values)
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

    _resourcesById.Clear();
    _loaders.Clear();
    _watchers.Clear();
  }

  /// <summary>
  /// Attempts to locate a valid loader for the given type.
  /// </summary>
  private bool TryGetLoader(ResourceContext context, [NotNullWhen(true)] out IResourceLoader? result)
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
/// Denotes the given resource type is not supported by the manager.
/// </summary>
public sealed class UnsupportedResourceException : Exception
{
  public UnsupportedResourceException(string message)
    : base(message)
  {
  }
}
