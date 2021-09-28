using System;
using System.Threading.Tasks;
using Surreal.IO;

namespace Surreal.Assets
{
  /// <summary>Allows loading assets from storage.</summary>
  public interface IAssetLoader
  {
    Type AssetType { get; }

    Task<object> LoadAsync(Path path, IAssetResolver context);
  }

  /// <summary>Base class for any <see cref="IAssetLoader"/> implementation.</summary>
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
