using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Context for asset loading operations.</summary>
public interface IAssetContext
{
	Asset<T> LoadAsset<T>(VirtualPath path)
		where T : class;
}
