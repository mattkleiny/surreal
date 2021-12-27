using System.Runtime.CompilerServices;
using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Possible statuses for an <see cref="Asset{T}"/>.</summary>
public enum AssetStatus
{
	Unknown,
	Unloaded,
	Loading,
	Ready
}

/// <summary>Represents uniquely some asset type at a given path.</summary>
public readonly record struct AssetId(Type Type, VirtualPath Path)
{
	public override string ToString() => Path.ToString();
}

/// <summary>Describes an asset in the asset manager with an opaque token.</summary>
public readonly record struct Asset<T>(AssetId Id, IAssetManager Manager) : IDisposable
	where T : class
{
	public AssetStatus Status => Manager.GetStatus(Id);
	public bool IsUnloaded => Status == AssetStatus.Unloaded;
	public bool IsLoading => Status == AssetStatus.Loading;
	public bool IsReady => Status == AssetStatus.Ready;

	public AssetAwaiter<T> GetAwaiter() => new(this);

	public void Unload() => Manager.Unload(Id);
	void IDisposable.Dispose() => Manager.Unload(Id);
}

/// <summary>Allows waiting until an <see cref="Asset{T}"/> has finished loading.</summary>
public readonly struct AssetAwaiter<T> : INotifyCompletion
	where T : class
{
	private readonly Asset<T> asset;

	public AssetAwaiter(Asset<T> asset)
	{
		this.asset = asset;
	}

	public bool IsCompleted => asset.IsReady;

	public T GetResult() => (T) asset.Manager.GetData(asset.Id)!;
	public void OnCompleted(Action continuation) => asset.Manager.AddCallback(asset.Id, continuation);
}
