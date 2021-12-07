using Path = Surreal.IO.Path;

namespace Surreal.Assets;

public interface IAssetResolver
{
  Asset<T> LoadAsset<T>(Path path)
    where T : class;
}
