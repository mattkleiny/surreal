using Surreal.Graphics;
using Surreal.Mathematics;

namespace Surreal.Terminals;

/// <summary>A terminal for <see cref="Glyph"/>-based rogue-like games.</summary>
public abstract class Terminal
{
  /// <summary>The width of the terminal, in columns.</summary>
  public abstract int Width { get; }

  /// <summary>The height of the terminal, in rows.</summary>
  public abstract int Height { get; }

  /// <summary>The default background color, when a color is not specified.</summary>
  public Color32 BackgroundColor { get; set; } = Color.Black;

  /// <summary>The default foreground color, when a color is not specified.</summary>
  public Color32 ForegroundColor { get; set; } = Color.White;

  /// <summary>Writes a single character to the terminal.</summary>
  public void WriteAt(int x, int y, char symbol, Color32? foregroundColor = default, Color32? backgroundColor = default)
  {
    var glyph = new Glyph(
      Symbol: symbol,
      ForegroundColor: foregroundColor.GetValueOrDefault(ForegroundColor),
      BackgroundColor: backgroundColor.GetValueOrDefault(BackgroundColor)
    );

    DrawGlyph(x, y, glyph);
  }

  /// <summary>Writes a string of characters to the terminal.</summary>
  public void WriteAt(int x, int y, string text, Color32? foregroundColor = default, Color32? backgroundColor = default)
  {
    for (var i = 0; i < text.Length; i++)
    {
      if (x + y >= Width) break; // we reached the end of the terminal

      var glyph = new Glyph(
        Symbol: text[i],
        ForegroundColor: foregroundColor.GetValueOrDefault(ForegroundColor),
        BackgroundColor: backgroundColor.GetValueOrDefault(BackgroundColor)
      );

      DrawGlyph(x + i, y, in glyph);
    }
  }

  /// <summary>Fills the given rectangle with the given color.</summary>
  public void Fill(int x, int y, int width, int height, Color32? color = null)
  {
    var glyph = new Glyph(
      Symbol: ' ',
      ForegroundColor: ForegroundColor,
      BackgroundColor: color.GetValueOrDefault(BackgroundColor)
    );

    for (var py = y; py < y + height; py++)
    for (var px = x; px < x + width; px++)
    {
      DrawGlyph(px, py, glyph);
    }
  }

  /// <summary>Clears the contents of the terminal.</summary>
  public void Clear()
  {
    Fill(0, 0, Width, Height);
  }

  /// <summary>Gets a <see cref="TerminalSlice"/> representing a sub-area of this terminal.</summary>
  public virtual TerminalSlice Slice(int x, int y, int width, int height)
  {
    return new TerminalSlice(x, y, width, height, this);
  }

  /// <summary>Draws a single glyph to the terminal.</summary>
  public abstract void DrawGlyph(int x, int y, in Glyph glyph);
}

/// <summary>A <see cref="Terminal"/> constrained to a sub area of the parent <see cref="Terminal"/>.</summary>
public sealed class TerminalSlice : Terminal
{
  private readonly int offsetX;
  private readonly int offsetY;
  private readonly Terminal terminal;

  public TerminalSlice(int offsetX, int offsetY, int width, int height, Terminal terminal)
  {
    this.offsetX  = offsetX;
    this.offsetY  = offsetY;
    this.terminal = terminal;

    Width  = width;
    Height = height;
  }

  public override int Width  { get; }
  public override int Height { get; }

  public override void DrawGlyph(int x, int y, in Glyph glyph)
  {
    if (x < 0 || x >= Width) return;
    if (y < 0 || y >= Height) return;

    terminal.DrawGlyph(offsetX + x, offsetY + y, in glyph);
  }

  public override TerminalSlice Slice(int x, int y, int width, int height)
  {
    // flatten sub-slices
    return new TerminalSlice(offsetX + x, offsetY + y, width, height, terminal);
  }
}

/// <summary>A <see cref="Terminal"/> that renders to the attached <see cref="Console"/>.</summary>
public sealed class ConsoleTerminal : Terminal
{
  public override int Width  => Console.BufferWidth;
  public override int Height => Console.BufferHeight;

  public override void DrawGlyph(int x, int y, in Glyph glyph)
  {
    Console.SetCursorPosition(x, y);
    Console.Write(glyph.Symbol);
  }
}

/// <summary>A <see cref="Terminal"/> that renders to a <see cref="IGraphicsServer"/>.</summary>
public sealed class GraphicsTerminal : Terminal, IGlyphRenderer
{
  private readonly IGraphicsServer server;
  private readonly GlyphCanvas canvas;

  public GraphicsTerminal(IGraphicsServer server, int width, int height)
  {
    this.server = server;

    canvas = new(width, height, this);
  }

  public override int Width  => canvas.Width;
  public override int Height => canvas.Height;

  public override void DrawGlyph(int x, int y, in Glyph glyph)
  {
    canvas[x, y] = glyph;
  }

  /// <summary>Flush the terminal to the graphics device.</summary>
  public void Flush()
  {
    canvas.Flush();
  }

  void IGlyphRenderer.RenderGlyph(int x, int y, in Glyph glyph)
  {
    // TODO: render glyphs using bitmap fonts
  }
}
