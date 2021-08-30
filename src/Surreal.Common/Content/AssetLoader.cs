using System;
using System.Threading.Tasks;
using Surreal.IO;

namespace Surreal.Content
{
  public interface IAssetLoader
  {
    Type AssetType { get; }

    Task<object> LoadAsync(Path path, IAssetResolver context);
  }

  public abstract class AssetLoader<T> : IAssetLoader
  {
    public virtual Type AssetType { get; } = typeof(T);

    public abstract Task<T> LoadAsync(Path path, IAssetResolver resolver);

    async Task<object> IAssetLoader.LoadAsync(Path path, IAssetResolver resolver)
    {
      return (await LoadAsync(path, resolver))!;
    }
  }
}
