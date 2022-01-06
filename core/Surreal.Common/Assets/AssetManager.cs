using System.Diagnostics.CodeAnalysis;
using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Allows managing assets.</summary>
public interface IAssetManager
{
  void AddLoader(IAssetLoader loader);

  Task<object> LoadAsset(Type type, VirtualPath path, CancellationToken cancellationToken = default);
  Task<T>      LoadAsset<T>(VirtualPath path, CancellationToken cancellationToken = default);
}

/// <summary>The default <see cref="IAssetManager"/> implementation.</summary>
public sealed class AssetManager : IAssetManager, IDisposable
{
  private readonly List<IAssetLoader>          loaders    = new();
  private readonly Dictionary<AssetId, object> assetsById = new();

  public void AddLoader(IAssetLoader loader)
  {
    loaders.Add(loader);
  }

  public async Task<object> LoadAsset(Type type, VirtualPath path, CancellationToken cancellationToken = default)
  {
    var context = new AssetLoaderContext
    {
      Path      = path,
      AssetType = type,
      Manager   = this,
    };

    var assetId = new AssetId(context.AssetType, path);

    if (!TryGetLoader(context, out var loader))
    {
      throw new UnsupportedAssetException($"An unsupported asset type was requested: {context.AssetType.Name}");
    }

    if (!assetsById.TryGetValue(assetId, out var asset))
    {
      assetsById[assetId] = asset = await loader.LoadAsync(context, cancellationToken);
    }

    return asset;
  }

  public async Task<T> LoadAsset<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    return (T) await LoadAsset(typeof(T), path, cancellationToken);
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
