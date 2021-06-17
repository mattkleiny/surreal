using System;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Data;
using Surreal.Data.VFS;

namespace Surreal.Modules.Fonts {
  public sealed class TrueTypeFont {
    public sealed class Loader : AssetLoader<TrueTypeFont> {
      public override async Task<TrueTypeFont> LoadAsync(Path path, IAssetLoaderContext context) {
        await using var stream = await path.OpenInputStreamAsync();

        throw new NotImplementedException();
      }
    }
  }
}