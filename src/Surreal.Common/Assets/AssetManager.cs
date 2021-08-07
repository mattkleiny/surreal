using System;
using System.Collections.Generic;
using System.Threading;
using Surreal.IO;

namespace Surreal.Assets
{
  public enum AssetStatus
  {
    Unknown,
    Unloaded,
    Loading,
    Ready,
  }

  public interface IAssetManager : IAssetResolver
  {
    void        AddLoader(IAssetLoader loader);
    void        AddCallback(AssetId id, Action callback);
    AssetStatus GetStatus(AssetId id);
    object?     GetData(AssetId id);
    void        Unload(AssetId id);
  }

  public sealed class AssetManager : IDisposable, IAssetManager
  {
    private readonly Dictionary<Type, IAssetLoader> loadersByType = new();
    private readonly Dictionary<AssetId, Entry>     assetsById    = new();

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

      var assetId = new AssetId(typeof(T), path);

      if (!assetsById.TryGetValue(assetId, out var entry))
      {
        assetsById[assetId] = entry = new Entry(assetId);

        entry.OnLoad();

        loader.LoadAsync(path, this).ContinueWith(task =>
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

  public class UnsupportedAssetException : Exception
  {
    public UnsupportedAssetException(string message)
        : base(message)
    {
    }
  }
}