using System;
using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Fonts {
  public sealed class BitmapFont {
    private readonly Glyph[] glyphs;

    public BitmapFont(Glyph[] glyphs) {
      this.glyphs = glyphs;
    }

    public bool TryGetGlyph(char symbol, out Glyph glyph) {
      glyph = default;

      for (var i = 0; i < glyphs.Length; i++) {
        glyph = glyphs[i];

        if (glyph.Symbol == symbol) {
          return true;
        }
      }

      return false;
    }

    public Area Measure(ReadOnlySpan<char> text, float scale = 1f) {
      var width  = 0f;
      var height = 0f;

      for (var i = 0; i < text.Length; i++) {
        var symbol = text[i];

        if (TryGetGlyph(symbol, out var glyph)) {
          width += glyph.Size.X * scale;

          var scaledHeight = glyph.Size.Y * scale;
          if (scaledHeight > height) {
            height = scaledHeight;
          }
        }
      }

      return new Area(width, height);
    }

    public readonly struct Glyph {
      public Glyph(char symbol, TextureRegion texture, Vector2 bearing) {
        Symbol  = symbol;
        Region  = texture;
        Bearing = bearing;
      }

      public char          Symbol  { get; }
      public TextureRegion Region  { get; }
      public Vector2       Bearing { get; }
      public Vector2       Size    => new(Region.Width, Region.Height);
    }

    public sealed class Loader : AssetLoader<BitmapFont> {
      private readonly CharacterSet characterSet;

      public Loader(CharacterSet characterSet) {
        this.characterSet = characterSet;
      }

      public override Task<BitmapFont> LoadAsync(Path path, IAssetLoaderContext context) {
        // TODO: actually implement me
        return Task.FromResult(new BitmapFont(Array.Empty<Glyph>()));
      }
    }
  }
}