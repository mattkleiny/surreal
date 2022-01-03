using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Allows managing assets.</summary>
public interface IAssetManager : IAssetContext
{
  void AddLoader(IAssetLoader loader);
}

/// <summary>The default <see cref="IAssetManager"/> implementation.</summary>
public sealed class AssetManager : IAssetManager, IDisposable
{
  private readonly Dictionary<Type, IAssetLoader> loadersByType = new();
  private readonly Dictionary<AssetId, object>    assetsById    = new();

  public void AddLoader(IAssetLoader loader)
  {
    loadersByType[loader.AssetType] = loader;
  }

  public async Task<T> LoadAsset<T>(VirtualPath path, CancellationToken cancellationToken = default)
    where T : class
  {
    if (!loadersByType.TryGetValue(typeof(T), out var loader))
    {
      throw new UnsupportedAssetException($"An unsupported asset type was requested: {typeof(T).Name}");
    }

    var assetId = new AssetId(typeof(T), path);

    if (!assetsById.TryGetValue(assetId, out var asset))
    {
      assetsById[assetId] = asset = await loader.LoadAsync(path, this, cancellationToken);
    }

    return (T) asset;
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

    foreach (var loader in loadersByType.Values)
    {
      if (loader is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    assetsById.Clear();
  }

  /// <summary>Represents uniquely some asset type at a given path.</summary>
  private readonly record struct AssetId(Type Type, VirtualPath Path)
  {
    public override string ToString() => Path.ToString();
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
