using Surreal.IO;

namespace Surreal.Framework.Screens {
  public interface IAssetManifest {
    void Add<TAsset>(Path path);
  }
}