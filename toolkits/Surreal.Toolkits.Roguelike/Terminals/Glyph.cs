using Surreal.Mathematics;

namespace Surreal.Terminals;

/// <summary>A coloured character that can be painted.</summary>
public readonly record struct Glyph(
  char Character,
  Color32 ForegroundColor,
  Color32 BackgroundColor
);

/// <summary>A canvas of <see cref="Glyph"/>s that manages dirty glyph positions.</summary>
public sealed class GlyphCanvas
{
  /// <summary>A callback for rendering individual glyphs to a terminal.</summary>
  public delegate void RenderGlyphCallback(int x, int y, in Glyph glyph);

  private readonly Grid<Glyph>  glyphs;
  private readonly Grid<Glyph?> changedGlyphs;

  public GlyphCanvas(int width, int height)
  {
    glyphs        = new Grid<Glyph>(width, height);
    changedGlyphs = new Grid<Glyph?>(width, height);

    Width  = width;
    Height = height;
  }

  public int Width  { get; }
  public int Height { get; }

  public void SetGlyph(int x, int y, in Glyph glyph)
  {
    if (x < 0 || x >= Width) return;
    if (y < 0 || y >= Height) return;

    if (glyphs[x, y] != glyph)
    {
      changedGlyphs[x, y] = glyph;
    }
    else
    {
      changedGlyphs[x, y] = null;
    }
  }

  public void Render(RenderGlyphCallback callback)
  {
    for (var y = 0; y < Height; y++)
    for (var x = 0; x < Width; x++)
    {
      // Only draw glyphs that are different since the last call.
      var glyph = changedGlyphs[x, y];
      if (glyph == null) continue;

      callback(x, y, glyph.Value);

      // It's up to date now.
      glyphs[x, y]        = glyph.Value;
      changedGlyphs[x, y] = null;
    }
  }
}
