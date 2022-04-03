using Surreal.Mathematics;

namespace Surreal.Terminals;

/// <summary>A coloured character that can be painted.</summary>
public readonly record struct Glyph(char Symbol, Color32 ForegroundColor, Color32 BackgroundColor);

/// <summary>A renderer for the <see cref="GlyphCanvas"/>.</summary>
public interface IGlyphRenderer
{
  /// <summary>Renders a particular glyph at the given X, Y position.</summary>
  void RenderGlyph(int x, int y, in Glyph glyph);
}

/// <summary>A canvas of <see cref="Glyph"/>s that manages dirty glyph positions.</summary>
public sealed class GlyphCanvas
{
  private readonly IGlyphRenderer renderer;
  private readonly Grid<Glyph> glyphs;
  private readonly Grid<Glyph?> dirtyGlyphs;

  public GlyphCanvas(int width, int height, IGlyphRenderer renderer)
  {
    this.renderer = renderer;

    glyphs = new Grid<Glyph>(width, height);
    dirtyGlyphs = new Grid<Glyph?>(width, height);

    Width = width;
    Height = height;
  }

  public int Width  { get; }
  public int Height { get; }

  public Glyph this[int x, int y]
  {
    get
    {
      if (x < 0 || x >= Width) return default;
      if (y < 0 || y >= Height) return default;

      return glyphs[x, y];
    }
    set
    {
      if (x < 0 || x >= Width) return;
      if (y < 0 || y >= Height) return;

      if (glyphs[x, y] != value)
      {
        dirtyGlyphs[x, y] = value;
      }
      else
      {
        dirtyGlyphs[x, y] = null;
      }
    }
  }

  /// <summary>Flushes changes to the canvas down to the underlying <see cref="IGlyphRenderer"/>.</summary>
  public void Flush()
  {
    for (var y = 0; y < Height; y++)
    for (var x = 0; x < Width; x++)
    {
      // Only draw glyphs that are dirty
      var glyph = dirtyGlyphs[x, y];
      if (glyph != null)
      {
        renderer.RenderGlyph(x, y, glyph.Value);

        // It's up to date now.
        glyphs[x, y] = glyph.Value;
        dirtyGlyphs[x, y] = null;
      }
    }
  }
}
