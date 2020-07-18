using System;
using System.Threading.Tasks;
using Surreal.IO;

namespace Surreal.Assets {
  public interface IAssetLoader {
    Type AssetType { get; }

    Task<object> LoadAssetAsync(Path path, IAssetLoaderContext context);
  }

  public interface IAssetLoaderContext : IAssetResolver {
  }

  public abstract class AssetLoader<TAsset> : IAssetLoader
      where TAsset : class {
    public virtual Type AssetType { get; } = typeof(TAsset);

    public abstract Task<TAsset> LoadAsync(Path path, IAssetLoaderContext context);

    async Task<object> IAssetLoader.LoadAssetAsync(Path path, IAssetLoaderContext context) {
      return await LoadAsync(path, context);
    }
  }
}