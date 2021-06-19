using System;
using System.Runtime.CompilerServices;

namespace Surreal.Assets
{
  public readonly struct AssetAwaiter<T> : INotifyCompletion
      where T : class
  {
    private readonly Asset<T> asset;

    public AssetAwaiter(Asset<T> asset)
    {
      this.asset = asset;
    }

    public bool IsCompleted => asset.IsReady;

    public Asset<T> GetResult()
    {
      return asset;
    }

    public void OnCompleted(Action continuation)
    {
      asset.Manager.AddCallback(asset.Id, continuation);
    }
  }
}