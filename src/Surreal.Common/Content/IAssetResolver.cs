using Surreal.Fibers;
using Surreal.IO;

namespace Surreal.Content
{
  public interface IAssetResolver
  {
    Asset<T> LoadAsset<T>(Path path)
        where T : class;

    FiberTask WaitOnAssets();
  }
}
