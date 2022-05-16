using Surreal.Graphics.Images;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Textures;

/// <summary>A utility for building texture atlases from raw pixels.</summary>
public class TextureAtlasBuilder
{
  private readonly Queue<Cell> cells = new();

  /// <summary>Adds a new cell to a <see cref="TextureAtlasBuilder"/>.</summary>
  public Cell AddCell(int width, int height)
  {
    var cell = new Cell(width, height);

    cells.Enqueue(cell);

    return cell;
  }

  /// <summary>Converts the <see cref="TextureAtlasBuilder"/> to a grid of <see cref="Color"/>.</summary>
  public Grid<Color32> ToGrid(int stride)
  {
    var totalWidth = cells.Sum(_ => _.Width);
    var maxHeight = cells.Max(_ => _.Height);

    var width = totalWidth / stride;
    var height = maxHeight * (cells.Count / stride);

    var result = new Grid<Color32>(width, height);

    ToSpan(result.Span);

    return result;
  }

  /// <summary>Converts the <see cref="TextureAtlasBuilder"/> to a single <see cref="Image"/>.</summary>
  public Image ToImage(int stride)
  {
    var totalWidth = cells.Sum(_ => _.Width);
    var maxHeight = cells.Max(_ => _.Height);

    var width = totalWidth / stride;
    var height = maxHeight * (cells.Count / stride);

    var image = new Image(width, height);

    ToSpan(image.Pixels);

    return image;
  }

  /// <summary>Converts the <see cref="TextureAtlasBuilder"/> to a single <see cref="Texture"/>.</summary>
  public Texture ToTexture(IGraphicsServer server, int stride)
  {
    var texture = new Texture(server, TextureFormat.Rgba8);
    var grid = ToGrid(stride);

    texture.WritePixels<Color32>(grid.Width, grid.Height, grid.Span);

    return texture;
  }

  /// <summary>Converts the result and writes to the given <see cref="SpanGrid{T}"/>.</summary>
  private void ToSpan(SpanGrid<Color32> target)
  {
    var offsetX = 0;
    var offsetY = 0;

    while (cells.TryDequeue(out var cell))
    {
      var source = cell.Span;

      for (int y = 0; y < cell.Height; y++)
      {
        for (int x = 0; x < cell.Width; x++)
        {
          target[offsetX + x, offsetY + y] = source[x, y];
        }
      }

      offsetX = (offsetX + cell.Width) % target.Width;
      offsetY = (offsetY + cell.Height) % target.Height;
    }
  }

  /// <summary>A single cell in a <see cref="TextureAtlasBuilder"/>.</summary>
  public readonly struct Cell
  {
    private readonly Grid<Color32> pixels;

    public Cell(int width, int height)
    {
      pixels = new Grid<Color32>(width, height);
    }

    public int Width  => pixels.Width;
    public int Height => pixels.Height;

    public SpanGrid<Color32> Span => pixels.Span;
  }
}
