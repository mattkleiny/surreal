using Surreal.Fibers;
using Surreal.IO;

namespace Surreal.Assets
{
  public interface IAssetResolver
  {
    Asset<T> LoadAsset<T>(Path path)
        where T : class;

    FiberTask WaitOnAssets();
  }
}
