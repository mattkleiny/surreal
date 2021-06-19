using System;

namespace Surreal.Assets
{
  public readonly struct Asset<T> : IDisposable
      where T : class
  {
    internal Asset(AssetId id, IAssetManager manager)
    {
      Id      = id;
      Manager = manager;
    }

    public AssetId       Id      { get; }
    public IAssetManager Manager { get; }

    public bool IsUnloaded => Manager.GetStatus(Id) == AssetStatus.Unloaded;
    public bool IsLoading  => Manager.GetStatus(Id) == AssetStatus.Loading;
    public bool IsReady    => Manager.GetStatus(Id) == AssetStatus.Ready;

    public T Data => (T) Manager.GetData(Id)!;

    public AssetAwaiter<T> GetAwaiter() => new(this);

    public void Dispose() => Manager.Unload(Id);
  }
}