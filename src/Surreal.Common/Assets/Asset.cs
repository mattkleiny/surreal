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

    public bool        IsValid    => Id.IsValid;
    public AssetStatus Status     => Manager.GetStatus(Id);
    public bool        IsUnloaded => Status == AssetStatus.Unloaded;
    public bool        IsLoading  => Status == AssetStatus.Loading;
    public bool        IsReady    => Status == AssetStatus.Ready;

    public T Data => (T) Manager.GetData(Id)!;

    public AssetAwaiter<T> GetAwaiter() => new(this);

    public void Dispose()
    {
      Manager.Unload(Id);
    }
  }
}