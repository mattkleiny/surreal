using System.Collections.Generic;
using Surreal.Mathematics;

namespace Surreal.Graphics.Textures {
  public sealed class TextureRegion : ICanSubdivide<TextureRegion> {
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
      return new TextureRegion(Texture, OffsetX + offsetX, OffsetY + offsetY, width, height);
    }

    public IEnumerable<TextureRegion> Subdivide(int regionWidth, int regionHeight) {
      var regionsX = Width  / regionWidth;
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

    public static implicit operator TextureRegion(Texture texture) => new TextureRegion(texture);
  }
}