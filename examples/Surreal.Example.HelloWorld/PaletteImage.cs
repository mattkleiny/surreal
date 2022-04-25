using Surreal.Graphics.Images;
using Surreal.Memory;

namespace HelloWorld;

/// <summary>An image of palette indices.</summary>
internal sealed class PaletteImage
{
  private IBuffer<uint> indices;

  public PaletteImage(int width, int height)
  {
    indices = Buffers.AllocatePinned<uint>(width * height);

    Width  = width;
    Height = height;
  }

  public int Width  { get; }
  public int Height { get; }

  public void DrawCircle(Point2 center, int radius, uint index)
  {
    var indices = this.indices.Data.Span;

    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var position = new Point2(x, y);
      var distance = position - center;

      if (distance.LengthSquared() <= radius)
      {
        indices[x + y * Width] = index;
      }
    }
  }

  public void DrawBox(Rectangle rectangle, uint index)
  {
    var indices = this.indices.Data.Span;

    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var position = new Point2(x, y);

      if (rectangle.Contains(position))
      {
        indices[x + y * Width] = index;
      }
    }
  }

  public void Fill(uint index)
  {
    indices.Data.Span.Fill(index);
  }

  public void CopyTo(Image image, ColorPalette palette)
  {
    var indices = this.indices.Data.Span;
    var pixels = image.Pixels;

    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var index = (int) indices[x + y * Width];

      pixels[x, y] = palette[index];
    }
  }
}
