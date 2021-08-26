using System;

namespace Surreal.Content
{
  public readonly record struct Asset<T>(AssetId Id, IAssetManager Manager) : IDisposable
      where T : class
  {
    public AssetStatus Status     => Manager.GetStatus(Id);
    public bool        IsUnloaded => Status == AssetStatus.Unloaded;
    public bool        IsLoading  => Status == AssetStatus.Loading;
    public bool        IsReady    => Status == AssetStatus.Ready;

    public T Data => (T)Manager.GetData(Id)!;

    public AssetAwaiter<T> GetAwaiter() => new(this);

    public void Dispose()
    {
      Manager.Unload(Id);
    }
  }
}
