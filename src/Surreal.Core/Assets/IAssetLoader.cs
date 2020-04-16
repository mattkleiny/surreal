using System;
using System.Threading.Tasks;
using Surreal.IO;

namespace Surreal.Assets
{
  public interface IAssetLoader
  {
    Type AssetType { get; }

    Task<object> LoadAssetAsync(Path path, IAssetLoaderContext context);
  }
}
