using Surreal.Graphics.Images;
using Surreal.Memory;

namespace HelloWorld.Graphics;

/// <summary>An image of palette indices.</summary>
internal sealed class PaletteImage
{
  private readonly IBuffer<uint> indices;

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
    var grid = indices.Span.ToGrid(Width);

    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var position = new Point2(x, y);
      var distance = position - center;

      if (distance.LengthSquared() <= radius)
      {
        grid[x, y] = index;
      }
    }
  }

  public void DrawBox(Rectangle rectangle, uint index)
  {
    var grid = indices.Span.ToGrid(Width);

    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var position = new Point2(x, y);

      if (rectangle.Contains(position))
      {
        grid[x, y] = index;
      }
    }
  }

  public void Fill(uint index)
  {
    indices.Data.Span.Fill(index);
  }

  public void CopyTo(Image image, ColorPalette palette)
  {
    var input = indices.Span.ToGrid(Width);
    var output = image.Pixels;

    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var index = (int) input[x, y];

      output[x, y] = palette[index];
    }
  }
}
