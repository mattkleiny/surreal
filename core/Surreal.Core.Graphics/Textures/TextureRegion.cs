using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Data;

namespace Surreal.Graphics.Textures {
  public sealed class TextureRegion : IDisposable {
    public TextureRegion(Texture texture)
        : this(texture, 0, 0, texture.Width, texture.Height) {
    }

    public TextureRegion(Texture texture, int offsetX, int offsetY, int width, int height) {
      Texture = texture;
      OffsetX = offsetX;
      OffsetY = offsetY;
      Width   = width;
      Height  = height;
    }

    public Texture Texture { get; }
    public int     Width   { get; }
    public int     Height  { get; }
    public int     OffsetX { get; }
    public int     OffsetY { get; }

    public TextureRegion Slice(int offsetX, int offsetY, int width, int height) {
      return new(Texture, OffsetX + offsetX, OffsetY + offsetY, width, height);
    }

    public IEnumerable<TextureRegion> Subdivide(int regionWidth, int regionHeight) {
      var regionsX = Width / regionWidth;
      var regionsY = Height / regionHeight;

      for (var y = 0; y < regionsY; y++)
      for (var x = 0; x < regionsX; x++) {
        yield return new TextureRegion(
            texture: Texture,
            offsetX: OffsetX + x * regionWidth,
            offsetY: OffsetY + y * regionHeight,
            width: regionWidth,
            height: regionHeight
        );
      }
    }

    public void Dispose() {
      Texture.Dispose();
    }

    public sealed class Loader : AssetLoader<TextureRegion> {
      public override async Task<TextureRegion> LoadAsync(Path path, IAssetLoaderContext context) {
        var texture = await context.GetAsync<Texture>(path);

        return texture.ToRegion();
      }
    }
  }
}