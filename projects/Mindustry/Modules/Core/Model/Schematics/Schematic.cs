using System;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;

namespace Mindustry.Modules.Core.Model.Schematics {
  [Serializable]
  public sealed class Schematic {
    public static async Task<Schematic> LoadAsync(Path path) {
      return await path.DeserializeBinaryAsync<Schematic>();
    }

    public async Task SaveAsync(Path path) {
      await path.SerializeBinaryAsync(this);
    }

    public sealed class Loader : AssetLoader<Schematic> {
      public override async Task<Schematic> LoadAsync(Path path, IAssetLoaderContext context) {
        return await Schematic.LoadAsync(path);
      }
    }
  }
}