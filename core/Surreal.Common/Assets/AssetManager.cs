using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Represents uniquely some asset type at a given path.</summary>
public readonly record struct AssetId(Type Type, VirtualPath Path)
{
  public override string ToString() => Path.ToString();
}

/// <summary>An opaque handle to an asset in the asset system.</summary>
public readonly record struct Asset<T>(AssetId Id, Task<T> Task) : IDisposable
{
  public T    Value   => Task.Result;
  public bool IsReady => Task.IsCompleted;

  public TaskAwaiter<T> GetAwaiter()
  {
    return Task.GetAwaiter();
  }

  public void Dispose()
  {
    if (Task.IsCompleted && Task.Result is IDisposable disposable)
    {
      disposable.Dispose();
    }
  }

  public static implicit operator T(Asset<T> asset) => asset.Value;
}

/// <summary>Allows managing assets.</summary>
public interface IAssetManager : IDisposable
{
  void AddLoader(IAssetLoader loader);

  Asset<T> LoadAsset<T>(VirtualPath path, CancellationToken cancellationToken = default);
}

/// <summary>The default <see cref="IAssetManager"/> implementation.</summary>
public sealed class AssetManager : IAssetManager
{
  private readonly List<IAssetLoader> loaders = new();
  private readonly Dictionary<AssetId, object> assetsById = new();

  public void AddLoader(IAssetLoader loader)
  {
    loaders.Add(loader);
  }

  [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
  public Asset<T> LoadAsset<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    async Task<T> StartLoadingAsset(IAssetLoader loader, AssetLoaderContext context)
    {
      var id = context.Id;

      if (!assetsById.TryGetValue(id, out var asset))
      {
        // we'll continue asynchronously on the main thread
        assetsById[id] = asset = await loader.LoadAsync(context, cancellationToken);
      }

      return (T) asset;
    }

    var id = new AssetId(typeof(T), path);
    var context = new AssetLoaderContext(id, this);

    if (!TryGetLoader(context, out var loader))
    {
      throw new UnsupportedAssetException($"An unsupported asset type was requested: {context.AssetType.Name}");
    }

    return new(id, StartLoadingAsset(loader, context));
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
