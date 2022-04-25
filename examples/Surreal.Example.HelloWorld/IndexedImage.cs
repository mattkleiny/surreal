using Surreal.Graphics.Images;

namespace HelloWorld;

public delegate Color32 Sampler(uint index);

public sealed class IndexedImage
{
  private readonly uint[] indices;

  public IndexedImage(int width, int height)
  {
    indices = new uint[width * height];

    Width  = width;
    Height = height;
  }

  public int Width  { get; }
  public int Height { get; }

  public void Blit(Image image, Sampler sampler)
  {
    var pixels = image.Pixels;

    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var index = indices[x + y * Width];

      pixels[x, y] = sampler(index);
    }
  }

  public void DrawCircle(Point2 center, int radius, uint index)
  {
    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var position = new Point2(x, y);

      if ((position - center).LengthSquared() <= radius)
      {
        indices[x + y * Width] = index;
      }
    }
  }

  public void DrawBox(Rectangle rectangle, uint index)
  {
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
    Array.Fill(indices, index);
  }
}
