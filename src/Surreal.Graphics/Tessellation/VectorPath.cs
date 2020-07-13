using System;
using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Tessellation {
  public sealed class VectorPath {
    public static async Task<VectorPath> LoadAsync(Path path) {
      await using var stream = await path.OpenInputStreamAsync();

      throw new NotImplementedException();
    }

    public Span<Vector2> Tesselate() {
      throw new NotImplementedException();
    }

    public sealed class Loader : AssetLoader<VectorPath> {
      public override async Task<VectorPath> LoadAsync(Path path, IAssetLoaderContext context) {
        return await VectorPath.LoadAsync(path);
      }
    }
  }
}