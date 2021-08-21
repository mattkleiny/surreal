using System;
using System.Threading.Tasks;
using Surreal.IO;

namespace Surreal.Assets
{
  public interface IAssetLoader
  {
    Type AssetType { get; }

    Task<object> LoadAsync(Path path, IAssetResolver context);
  }

  public abstract class AssetLoader<T> : IAssetLoader
      where T : class
  {
    public virtual Type AssetType { get; } = typeof(T);

    async Task<object> IAssetLoader.LoadAsync(Path path, IAssetResolver context)
    {
      return await LoadAsync(path, context);
    }

    public abstract Task<T> LoadAsync(Path path, IAssetResolver context);
  }
}
