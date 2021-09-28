using System;

namespace Surreal.Assets
{
  /// <summary>Possible statuses for an <see cref="Asset{T}"/>.</summary>
  public enum AssetStatus
  {
    Unknown,
    Unloaded,
    Loading,
    Ready
  }

  /// <summary>Describes an asset in the asset manager with an opaque token.</summary>
  public readonly record struct Asset<T>(AssetId Id, IAssetManager Manager) : IDisposable
      where T : class
  {
    public AssetStatus Status     => Manager.GetStatus(Id);
    public bool        IsUnloaded => Status == AssetStatus.Unloaded;
    public bool        IsLoading  => Status == AssetStatus.Loading;
    public bool        IsReady    => Status == AssetStatus.Ready;

    public T Data => (T) Manager.GetData(Id)!;

    public AssetAwaiter<T> GetAwaiter() => new(this);

    public void Unload() => Manager.Unload(Id);

    void IDisposable.Dispose() => Manager.Unload(Id);

    public static implicit operator T(Asset<T> asset) => asset.Data;
  }
}
