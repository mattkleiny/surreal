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

  private readonly Dictionary<AssetId, Entry> _entriesById = new();
  private readonly List<IAssetLoader> _loaders = new();
  private readonly List<IPathWatcher> _watchers = new();

  /// <summary>
  /// Determines if hot reloading assets are enabled.
  /// </summary>
  public bool IsHotReloadEnabled { get; init; } = true;

  public void AddLoader(IAssetLoader loader)
  {
    Log.Trace($"Registering asset loader {loader.GetType().Name}");

    _loaders.Add(loader);
  }

  public async Task<T> LoadAssetAsync<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    var id = new AssetId(typeof(T), path);
    var context = new AssetContext(id, this);

    if (!TryGetLoader(context, out var loader))
    {
      throw new UnsupportedAssetException($"An unsupported asset type was requested: {context.Type.Name}");
    }

    if (!_entriesById.TryGetValue(id, out var entry))
    {
      Log.Trace($"Loading asset {id.Path} as {id.Type.Name} using {loader.GetType().Name}");

      _entriesById[id] = entry = new Entry(await loader.LoadAsync(context, cancellationToken));
    }

    return (T)entry.Asset;
  }

  /// <summary>
  /// Watches for changes in the asset at the given path.
  /// </summary>
  public void WatchForChanges<T>(AssetId id, IHotReloadable<T> reloadable)
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
    foreach (var asset in _entriesById.Values)
    {
      if (asset is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    _entriesById.Clear();
    _loaders.Clear();
  }

  /// <summary>
  /// Attempts to locate a valid loader for the given type.
  /// </summary>
  private bool TryGetLoader(AssetContext context, [NotNullWhen(true)] out IAssetLoader? result)
  {
    foreach (var loader in _loaders)
    {
      if (loader.CanHandle(context))
      {
        result = loader;
        return true;
      }
    }

    // fallback to common loader
    var commonLoader = CommonAssetLoader.Instance;
    if (commonLoader.CanHandle(context))
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
  private sealed class Entry(object asset) : IDisposable
  {
    /// <summary>
    /// Invoked when the underlying asset has reloaded.
    /// </summary>
    public event Action<object>? Reloaded;

    /// <summary>
    /// The actual asset data.
    /// </summary>
    public object Asset { get; set; } = asset;

    /// <summary>
    /// Notifies that the asset has been reloaded.
    /// </summary>
    public void NotifyReloaded()
    {
      Reloaded?.Invoke(Asset);
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
  /// A <see cref="IAssetLoader"/> that can load any standard file format.
  /// </summary>
  private sealed class CommonAssetLoader : IAssetLoader
  {
    public static CommonAssetLoader Instance { get; } = new();

    public bool CanHandle(AssetContext context)
    {
      var extension = context.Path.Extension;

      return extension.EndsWith("json") ||
             extension.EndsWith("yml") ||
             extension.EndsWith("xml");
    }

    public async Task<object> LoadAsync(AssetContext context, CancellationToken cancellationToken)
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
