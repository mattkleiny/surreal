using System;
using System.Collections.Generic;
using System.Threading;
using Surreal.IO;

namespace Surreal.Assets
{
  /// <summary>Allows managing <see cref="Asset{T}"/>s.</summary>
  public interface IAssetManager
  {
    IAssetResolver CreateResolver();

    void        AddLoader(IAssetLoader loader);
    void        AddCallback(AssetId id, Action callback);
    AssetStatus GetStatus(AssetId id);
    object?     GetData(AssetId id);
    void        Unload(AssetId id);
  }

  /// <summary>The default <see cref="IAssetManager"/> implementation.</summary>
  public sealed class AssetManager : IAssetManager, IDisposable
  {
    private readonly Dictionary<Type, IAssetLoader> loadersByType = new();
    private readonly Dictionary<AssetId, Entry>     assetsById    = new();

    public IAssetResolver CreateResolver()
    {
      return new AssetResolver(this);
    }

    public void AddLoader(IAssetLoader loader)
    {
      loadersByType[loader.AssetType] = loader;
    }

    public void AddCallback(AssetId id, Action callback)
    {
      if (assetsById.TryGetValue(id, out var entry))
      {
        entry.Callbacks.Enqueue(callback);
      }
    }

    public Asset<T> LoadAsset<T>(Path path)
        where T : class
    {
      if (!loadersByType.TryGetValue(typeof(T), out var loader))
      {
        throw new UnsupportedAssetException($"An unsupported asset type was requested: {typeof(T).Name}");
      }

      var assetId  = new AssetId(typeof(T), path);
      var resolver = CreateResolver();

      if (!assetsById.TryGetValue(assetId, out var entry))
      {
        assetsById[assetId] = entry = new Entry(assetId);

        entry.OnLoad();

        loader.LoadAsync(path, resolver).ContinueWith(task =>
        {
          if (assetsById.TryGetValue(assetId, out entry))
          {
            entry.OnReady(task.Result);
          }
        });
      }

      return new Asset<T>(assetId, this);
    }

    public AssetStatus GetStatus(AssetId id)
    {
      if (assetsById.TryGetValue(id, out var entry))
      {
        return entry.Status;
      }

      return AssetStatus.Unloaded;
    }

    public object? GetData(AssetId id)
    {
      if (assetsById.TryGetValue(id, out var entry))
      {
        return entry.Data;
      }

      return default;
    }

    public void Unload(AssetId id)
    {
      if (assetsById.TryGetValue(id, out var entry))
      {
        entry.OnUnload();

        assetsById.Remove(id);
      }
    }

    public void Dispose()
    {
      foreach (var entry in assetsById.Values)
      {
        entry.OnUnload();
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

    private sealed class AssetResolver : IAssetResolver
    {
      private readonly AssetManager manager;

      public AssetResolver(AssetManager manager)
      {
        this.manager = manager;
      }

      public Asset<T> LoadAsset<T>(Path path)
          where T : class
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>A single loaded asset and a set of callbacks for observation.</summary>
    private sealed record Entry(AssetId Id)
    {
      public Queue<Action> Callbacks { get; }              = new(0);
      public AssetStatus   Status    { get; private set; } = AssetStatus.Unknown;
      public object?       Data      { get; set; }

      public void OnLoad()
      {
        Status = AssetStatus.Loading;
      }

      public void OnReady(object result)
      {
        Data   = result;
        Status = AssetStatus.Ready;

        while (Callbacks.TryDequeue(out var callback))
        {
          SynchronizationContext.Current?.Send(_ => callback(), null);
        }
      }

      public void OnUnload()
      {
        if (Data is IDisposable disposable)
        {
          disposable.Dispose();
        }

        Data   = default;
        Status = AssetStatus.Unloaded;
      }
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
}
