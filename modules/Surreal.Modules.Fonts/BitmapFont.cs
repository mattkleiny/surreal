using System;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Data;
using Surreal.Data.VFS;

namespace Surreal.Modules.Fonts {
  public sealed class BitmapFont {
    public sealed class Loader : AssetLoader<BitmapFont> {
      public override async Task<BitmapFont> LoadAsync(Path path, IAssetLoaderContext context) {
        await using var stream = await path.OpenInputStreamAsync();

        throw new NotImplementedException();
      }
    }
  }
}