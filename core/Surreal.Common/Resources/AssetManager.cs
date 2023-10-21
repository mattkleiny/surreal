using Surreal.Diagnostics.Logging;
using Surreal.IO;

namespace Surreal.Resources;

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

  private readonly Dictionary<AssetId, object> _assetsById = new();
  private readonly List<IAssetLoader> _loaders = new();

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

    if (!_assetsById.TryGetValue(id, out var asset))
    {
      Log.Trace($"Loading asset {id.Path}");

      _assetsById[id] = asset = await loader.LoadAsync(context, cancellationToken);
    }

    return (T)asset;
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

    _assetsById.Clear();
    _loaders.Clear();
  }

  /// <summary>
  /// Attempts to locate a valid loader for the given type.
  /// </summary>
  private bool TryGetLoader(AssetContext context, [NotNullWhen(true)] out IAssetLoader? result)
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
