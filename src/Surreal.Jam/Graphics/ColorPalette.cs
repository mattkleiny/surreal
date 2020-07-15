using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Path = Surreal.IO.Path;

namespace Surreal.Graphics {
  public sealed class ColorPalette : IDisposable {
    private readonly Color[] colors;

    private Image?   image;
    private Texture? texture;

    public ColorPalette(params Color[] colors) {
      this.colors = colors;
    }

    public int Count => colors.Length;
    public ref Color this[int index] => ref colors[index];

    public Image ToImage() {
      image ??= new Image(Count, 1);

      for (var i = 0; i < Count; i++) {
        image[i, 0] = this[i];
      }

      return image;
    }

    public Texture ToTexture(IGraphicsDevice device) {
      var image = ToImage();

      texture ??= device.CreateTexture(image, filterMode: TextureFilterMode.Point);
      texture.Upload(image);

      return texture;
    }

    public void Dispose() {
      image?.Dispose();
      texture?.Dispose();
    }

    public sealed class Loader : AssetLoader<ColorPalette> {
      public override async Task<ColorPalette> LoadAsync(Path path, IAssetLoaderContext context) {
        var palette = path.GetExtension().ToLower() switch {
            ".pal" => await ParseJascAsync(path),
            _      => throw new Exception($"An unrecognized palette format was requested: {path}")
        };

        return palette;
      }

      private static async Task<ColorPalette> ParseJascAsync(Path path) {
        await using var stream = await path.OpenInputStreamAsync();
        using var       reader = new StreamReader(stream);

        if (await reader.ReadLineAsync() != "JASC-PAL") {
          throw new Exception("An unrecognized palette format was encountered!");
        }

        if (await reader.ReadLineAsync() != "0100") {
          throw new Exception("An unrecognized palette format was encountered!");
        }

        var count  = int.Parse(await reader.ReadLineAsync());
        var colors = new Color[count];

        for (var i = 0; i < colors.Length; i++) {
          var line = await reader.ReadLineAsync();
          var raw = line.Split(' ')
              .Select(_ => _.Trim())
              .Select(byte.Parse)
              .ToArray();

          if (raw.Length != 3) {
            throw new Exception($"Expected 3 but received {raw.Length} color values!");
          }

          colors[i] = new Color(raw[0], raw[1], raw[2]);
        }

        return new ColorPalette(colors);
      }
    }
  }
}