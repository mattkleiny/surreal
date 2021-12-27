using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Allows loading assets from storage.</summary>
public interface IAssetLoader
{
	Type AssetType { get; }

	Task<object> LoadAsync(VirtualPath path, IAssetContext context);
}

/// <summary>Base class for any <see cref="IAssetLoader"/> implementation.</summary>
public abstract class AssetLoader<T> : IAssetLoader
{
	public virtual Type AssetType { get; } = typeof(T);

	public abstract Task<T> LoadAsync(VirtualPath path, IAssetContext context);

	async Task<object> IAssetLoader.LoadAsync(VirtualPath path, IAssetContext context)
	{
		return (await LoadAsync(path, context))!;
	}
}
