using Surreal.Graphics.Images;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Textures;

/// <summary>A utility for building texture atlases from raw pixels.</summary>
public class TextureAtlasBuilder
{
  // TODO: work on this

  private readonly Queue<Cell> _cells = new();

  /// <summary>Adds a new cell to a <see cref="TextureAtlasBuilder" />.</summary>
  public Cell AddCell(int width, int height)
  {
    var cell = new Cell(width, height);

    _cells.Enqueue(cell);

    return cell;
  }

  /// <summary>Converts the <see cref="TextureAtlasBuilder" /> to a grid of <see cref="Color" />.</summary>
  public Grid<Color32> ToGrid(int stride)
  {
    var totalWidth = _cells.Sum(_ => _.Width);
    var maxHeight = _cells.Max(_ => _.Height);

    var width = totalWidth / stride;
    var height = maxHeight * (_cells.Count / stride);

    var result = new Grid<Color32>(width, height);

    ToSpan(result.Span);

    return result;
  }

  /// <summary>Converts the <see cref="TextureAtlasBuilder" /> to a single <see cref="Image" />.</summary>
  public Image ToImage(int stride)
  {
    var totalWidth = _cells.Sum(_ => _.Width);
    var maxHeight = _cells.Max(_ => _.Height);

    var width = totalWidth / stride;
    var height = maxHeight * (_cells.Count / stride);

    var image = new Image(width, height);

    ToSpan(image.Pixels);

    return image;
  }

  /// <summary>Converts the <see cref="TextureAtlasBuilder" /> to a single <see cref="Texture" />.</summary>
  public Texture ToTexture(IGraphicsServer server, int stride)
  {
    var texture = new Texture(server);
    var grid = ToGrid(stride);

    texture.WritePixels<Color32>(grid.Width, grid.Height, grid.Span);

    return texture;
  }

  /// <summary>Converts the result and writes to the given <see cref="SpanGrid{T}" />.</summary>
  private void ToSpan(SpanGrid<Color32> target)
  {
    var offsetX = 0;
    var offsetY = 0;

    while (_cells.TryDequeue(out var cell))
    {
      var source = cell.Span;

      for (var y = 0; y < cell.Height; y++)
      for (var x = 0; x < cell.Width; x++)
        target[offsetX + x, offsetY + y] = source[x, y];

      offsetX = (offsetX + cell.Width) % target.Width;
      offsetY = (offsetY + cell.Height) % target.Height;
    }
  }

  /// <summary>A single cell in a <see cref="TextureAtlasBuilder" />.</summary>
  public readonly struct Cell
  {
    private readonly Grid<Color32> _pixels;

    public Cell(int width, int height)
    {
      _pixels = new Grid<Color32>(width, height);
    }

    public int Width => _pixels.Width;
    public int Height => _pixels.Height;

    public SpanGrid<Color32> Span => _pixels.Span;
  }
}



