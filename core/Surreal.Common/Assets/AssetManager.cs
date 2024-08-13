using Surreal.Collections;
using Surreal.Diagnostics.Logging;
using Surreal.IO;

namespace Surreal.Assets;

/// <summary>
/// Denotes the given asset type is not supported by the manager.
/// </summary>
public sealed class UnsupportedAssetException(string message) : ApplicationException(message);

/// <summary>
/// Represents uniquely some resource type at a given path.
/// </summary>
public readonly record struct AssetId(Type Type, VirtualPath Path)
{
  public override string ToString() => Path.ToString();
}

/// <summary>
/// The default <see cref="AssetManager" /> implementation.
/// </summary>
public sealed class AssetManager : IAssetProvider, IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<AssetManager>();

  private readonly Dictionary<AssetId, AssetEntry> _entriesById = new();
  private readonly MultiDictionary<VirtualPath, AssetEntry> _entriesByPath = new();
  private readonly List<IAssetLoader> _loaders = [];
  private readonly List<IPathWatcher> _watchers = [];

  /// <summary>
  /// Determines if hot reloading assets are enabled.
  /// </summary>
  public bool IsHotReloadEnabled { get; init; } = true;

  /// <summary>
  /// Watches for changes in assets at the given path.
  /// </summary>
  public void WatchAssets(VirtualPath path)
  {
    if (!IsHotReloadEnabled) return;

    if (!path.SupportsWatching())
    {
      Log.Warn($"The path {path} does not support watching, hot reloading will not work");
      return;
    }

    Log.Trace($"Watching paths at {path}");

    var watcher = path.Watch(includeSubPaths: true);

    watcher.Changed += OnPathChanged;

    _watchers.Add(watcher);
  }

  /// <summary>
  /// Adds an <see cref="IAssetLoader"/> to the manager.
  /// </summary>
  public void AddLoader(IAssetLoader loader)
  {
    Log.Trace($"Registering asset loader {loader.GetType().Name}");

    _loaders.Add(loader);
  }

  /// <inheritdoc/>
  public async Task<T> LoadAssetAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    var entry = await LoadEntryAsync<T>(path, cancellationToken);

    return (T)entry.Asset!; // by this point the asset should be loaded
  }

  /// <summary>
  /// Loads a dependency asset from the given path.
  /// </summary>
  private async Task<AssetEntry> LoadEntryAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    var assetId = new AssetId(typeof(T), path);

    if (!_entriesById.TryGetValue(assetId, out var entry))
    {
      if (!TryGetLoader(assetId, out var loader))
      {
        throw new UnsupportedAssetException($"An unsupported asset type was requested: {assetId.Type.Name}");
      }

      Log.Trace($"Loading asset {assetId.Path} as {assetId.Type.Name} using {loader.GetType().Name}");

      var normalizedPath = path.Normalize();

      // build a new entry
      entry = new AssetEntry(assetId);
      var context = new AssetContext(this, entry);

      _entriesById[assetId] = entry;
      _entriesByPath.Add(normalizedPath, entry);

      entry.Asset = await loader.LoadAsync(context, cancellationToken);
    }

    return entry;
  }

  public void Dispose()
  {
    // dispose all assets
    foreach (var asset in _entriesById.Values)
    {
      if (asset is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    // unregister all watchers
    foreach (var watcher in _watchers)
    {
      watcher.Changed -= OnPathChanged;
      watcher.Dispose();
    }

    _entriesById.Clear();
    _entriesByPath.Clear();
    _loaders.Clear();
    _watchers.Clear();
  }

  /// <summary>
  /// Invoked when a path has changed.
  /// </summary>
  private void OnPathChanged(VirtualPath path, PathChangeTypes type)
  {
    if (type.HasFlagFast(PathChangeTypes.Modified))
    {
      var normalizedPath = path.Normalize();

      foreach (var entry in _entriesByPath[normalizedPath])
      {
        entry.NotifyFileChanged();
      }
    }
  }

  /// <summary>
  /// Attempts to locate a valid loader for the given type.
  /// </summary>
  private bool TryGetLoader(AssetId assetId, [NotNullWhen(true)] out IAssetLoader? result)
  {
    foreach (var loader in _loaders)
    {
      if (loader.CanHandle(assetId))
      {
        result = loader;
        return true;
      }
    }

    // fallback to common loader
    var commonLoader = CommonAssetLoader.Instance;
    if (commonLoader.CanHandle(assetId))
    {
      result = commonLoader;
      return true;
    }

    result = default;
    return false;
  }

  /// <summary>
  /// An entry in the <see cref="AssetManager"/>.
  /// </summary>
  private sealed class AssetEntry(AssetId id) : IDisposable
  {
    public event Action? Changed;

    /// <summary>
    /// The ID of the asset.
    /// </summary>
    public AssetId Id => id;

    /// <summary>
    /// The actual asset data.
    /// </summary>
    public object? Asset { get; set; }

    /// <summary>
    /// Notifies that the asset path has been changed.
    /// </summary>
    public void NotifyFileChanged()
    {
      Log.Trace($"Asset changed at {Id.Path}");

      Changed?.Invoke();
    }

    public void Dispose()
    {
      if (Asset is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }
  }

  /// <summary>
  /// Context for <see cref="IAssetLoader"/> operations.
  /// </summary>
  private sealed record AssetContext(AssetManager Manager, AssetEntry Entry) : IAssetContext
  {
    public Type Type => Entry.Id.Type;
    public VirtualPath Path => Entry.Id.Path;

    public async Task<T> LoadAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
    {
      return await Manager.LoadAssetAsync<T>(path, cancellationToken);
    }

    public async Task<IAssetDependency<T>> LoadDependencyAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
    {
      return new AssetDependency<T>(await Manager.LoadEntryAsync<T>(path, cancellationToken));
    }

    /// <summary>
    /// Adds a reload action to the context.
    /// </summary>
    public void WhenPathChanged(ReloadCallback callback)
    {
      Entry.Changed += async () =>
      {
        try
        {
          // TODO: pass a cancellation token
          await callback(CancellationToken.None);
        }
        catch (Exception exception)
        {
          Log.Error(exception, "Failed to reload asset");
        }
      };
    }

    /// <summary>
    /// A <see cref="IAssetDependency{T}"/> implementation for the manager.
    /// </summary>
    private sealed record AssetDependency<T>(AssetEntry Entry) : IAssetDependency<T>
    {
      public T Value => (T)Entry.Asset!;

      public void WhenChanged(Action callback)
      {
        Entry.Changed += callback;
      }
    }
  }

  /// <summary>
  /// A <see cref="IAssetLoader"/> that can load any standard file format.
  /// </summary>
  private sealed class CommonAssetLoader : IAssetLoader
  {
    public static CommonAssetLoader Instance { get; } = new();

    public bool CanHandle(AssetId id)
    {
      var extension = id.Path.Extension;

      return extension.EndsWith("json") ||
             extension.EndsWith("yml") ||
             extension.EndsWith("xml");
    }

    public async Task<object> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
    {
      var format = context.Path.Extension switch
      {
        ".json" => FileFormat.Json,
        ".yml" => FileFormat.Yml,
        ".xml" => FileFormat.Xml,

        _ => throw new NotSupportedException($"Unsupported file extension: {context.Path.Extension}")
      };

      return await context.Path.DeserializeAsync(context.Type, format, cancellationToken);
    }
  }
}
