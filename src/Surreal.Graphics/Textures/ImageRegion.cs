using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Mathematics;
using Surreal.Mathematics.Grids;

namespace Surreal.Graphics.Textures {
  [DebuggerDisplay("Image Region {Width}x{Height} at ({OffsetX}, {OffsetY})")]
  public sealed class ImageRegion : IDisposable, IGrid<Color>, ICanSubdivide<ImageRegion>, ITextureData {
    public ImageRegion(Image image)
        : this(image, 0, 0, image.Width, image.Height) {
    }

    public ImageRegion(Image image, int offsetX, int offsetY, int width, int height) {
      Image   = image;
      OffsetX = offsetX;
      OffsetY = offsetY;
      Width   = width;
      Height  = height;
    }

    public Image Image   { get; }
    public int   Width   { get; }
    public int   Height  { get; }
    public int   OffsetX { get; }
    public int   OffsetY { get; }

    public TextureFormat Format => Image.Format;
    public Span<Color>   Span   => Image.Span.Slice(OffsetX * Width + OffsetY * Height, Width * Height);
    public Size          Size   => new Size(Width * Height * Unsafe.SizeOf<Color>());

    public Color this[int x, int y] {
      get => Image[OffsetX + x, OffsetY + y];
      set => Image[OffsetX + x, OffsetY + y] = value;
    }

    public void Fill(Color value) {
      Span.Fill(value);
    }

    public ImageRegion Slice(int offsetX, int offsetY, int width, int height) {
      return new ImageRegion(Image, OffsetX + offsetX, OffsetY + offsetY, width, height);
    }

    public IEnumerable<ImageRegion> Subdivide(int regionWidth, int regionHeight) {
      var regionsX = Width / regionWidth;
      var regionsY = Height / regionHeight;

      for (var y = 0; y < regionsY; y++)
      for (var x = 0; x < regionsX; x++) {
        yield return new ImageRegion(
            image: Image,
            offsetX: OffsetX + x * regionWidth,
            offsetY: OffsetY + y * regionHeight,
            width: regionWidth,
            height: regionHeight
        );
      }
    }

    public void Dispose() {
      Image.Dispose();
    }

    public sealed class Loader : AssetLoader<ImageRegion> {
      public override async Task<ImageRegion> LoadAsync(Path path, IAssetLoaderContext context) {
        var image = await context.GetAsync<Image>(path);

        return image.ToRegion();
      }
    }
  }
}