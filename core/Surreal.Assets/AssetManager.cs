using System;
using System.Collections.Generic;
using Surreal.IO;

namespace Surreal.Assets
{
  public enum AssetStatus
  {
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
    private readonly Dictionary<AssetId, AssetSlot> assetsById    = new();

    public void AddLoader(IAssetLoader loader)
    {
      loadersByType[loader.AssetType] = loader;
    }

    public void AddCallback(AssetId id, Action callback)
    {
      if (assetsById.TryGetValue(id, out var slot))
      {
        slot.Callbacks.Enqueue(callback);
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

      if (!assetsById.TryGetValue(assetId, out _))
      {
        assetsById[assetId] = new AssetSlot(assetId);

        loader.LoadAsync(path, this).ContinueWith(_ =>
        {
          if (assetsById.TryGetValue(assetId, out var slot))
          {
            slot.AssignData(_.Result);
          }
        });
      }

      return new Asset<T>(assetId, this);
    }

    public AssetStatus GetStatus(AssetId id)
    {
      if (assetsById.TryGetValue(id, out var slot))
      {
        return slot.Status;
      }

      return AssetStatus.Unloaded;
    }

    public object? GetData(AssetId id)
    {
      if (assetsById.TryGetValue(id, out var slot))
      {
        return slot.Data;
      }

      return default;
    }

    public void Unload(AssetId id)
    {
      if (assetsById.TryGetValue(id, out var slot))
      {
        if (slot.Data is IDisposable disposable)
        {
          disposable.Dispose();
        }

        assetsById.Remove(id);
      }
    }

    public void Dispose()
    {
      foreach (var slot in assetsById.Values)
      {
        if (slot.Data is IDisposable disposable)
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

    private sealed record AssetSlot(AssetId Id, AssetStatus Status = AssetStatus.Unloaded)
    {
      public object?       Data      { get; set; }
      public Queue<Action> Callbacks { get; } = new(0);

      public void AssignData(object result)
      {
        Data = result;

        while (Callbacks.TryDequeue(out var callback))
        {
          callback.Invoke();
        }
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