using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Surreal.Mathematics;
using Surreal.Mathematics.Grids;
using Surreal.Memory;

namespace Surreal.Graphics.Textures
{
  [DebuggerDisplay("Pixmap Region {Width}x{Height} at ({OffsetX}, {OffsetY})")]
  public sealed class PixmapRegion : IGrid<Color>, ICanSubdivide<PixmapRegion>, ITextureData
  {
    public PixmapRegion(Pixmap pixmap)
      : this(pixmap, 0, 0, pixmap.Width, pixmap.Height)
    {
    }

    public PixmapRegion(Pixmap pixmap, int offsetX, int offsetY, int width, int height)
    {
      Pixmap  = pixmap;
      OffsetX = offsetX;
      OffsetY = offsetY;
      Width   = width;
      Height  = height;
    }

    public Pixmap Pixmap  { get; }
    public int    Width   { get; }
    public int    Height  { get; }
    public int    OffsetX { get; }
    public int    OffsetY { get; }

    TextureFormat ITextureData.Format => Pixmap.Format;
    Span<Color> ITextureData.  Span   => Pixmap.Span.Slice(OffsetX * Width + OffsetY * Height);
    Size ITextureData.         Size   => new Size(Width * Height * Unsafe.SizeOf<Color>());

    public Color this[int x, int y]
    {
      get
      {
        if (x < 0 || x > Width - 1) return Color.Clear;
        if (y < 0 || y > Height - 1) return Color.Clear;

        return Pixmap[OffsetX + x, OffsetY + y];
      }
      set
      {
        if (x < 0 || x > Width - 1) return;
        if (y < 0 || y > Height - 1) return;

        Pixmap[OffsetX + x, OffsetY + y] = value;
      }
    }

    public PixmapRegion Slice(int offsetX, int offsetY, int width, int height)
    {
      return new PixmapRegion(Pixmap, OffsetX + offsetX, OffsetY + offsetY, width, height);
    }

    public IEnumerable<PixmapRegion> Subdivide(int regionWidth, int regionHeight)
    {
      var regionsX = Width / regionWidth;
      var regionsY = Height / regionHeight;

      for (var y = 0; y < regionsY; y++)
      for (var x = 0; x < regionsX; x++)
      {
        yield return new PixmapRegion(
          pixmap: Pixmap,
          offsetX: OffsetX + x * regionWidth,
          offsetY: OffsetY + y * regionHeight,
          width: regionWidth,
          height: regionHeight
        );
      }
    }

    public static implicit operator PixmapRegion(Pixmap pixmap) => new PixmapRegion(pixmap);
  }
}