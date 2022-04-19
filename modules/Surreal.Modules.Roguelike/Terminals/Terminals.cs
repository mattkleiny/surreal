using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
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
    this.offsetX = offsetX;
    this.offsetY = offsetY;
    this.terminal = terminal;

    Width = width;
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
[SupportedOSPlatform("windows")]
public sealed class ConsoleTerminal : Terminal, IDisposable
{
  private SafeFileHandle consoleHandle;
  private bool isDisposed;

  public ConsoleTerminal()
  {
    Initialize();
  }

  [STAThread]
  private void Initialize()
  {
    consoleHandle = Interop.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

    if (consoleHandle.IsInvalid)
    {
      throw new InvalidOperationException("Failed to acquire handle to attached console");
    }
  }

  ~ConsoleTerminal()
  {
    Dispose(managed: false);
  }

  public override int Width  => 180;
  public override int Height => 150;

  public override void DrawGlyph(int x, int y, in Glyph glyph)
  {
    var characters = new Interop.CharInfo[]
    {
      new()
      {
        Char = new Interop.CharUnion { AsciiChar = Convert.ToByte(glyph.Symbol) }
      }
    };

    var rect = new Interop.SmallRect
    {
      Left = (short)x,
      Top = (short)y,
      Right = (short)(x + 1),
      Bottom = (short)(y + 1)
    };

    Interop.WriteConsoleOutputW(consoleHandle, characters, new Interop.Coord { X = 1, Y = 1 }, new Interop.Coord { X = (short)x, Y = (short)y }, ref rect);
  }

  public void Dispose()
  {
    if (!isDisposed)
    {
      Dispose(true);

      GC.SuppressFinalize(this);
      isDisposed = true;
    }
  }

  private void Dispose(bool managed)
  {
    Interop.CloseHandle(consoleHandle);
  }

  private static class Interop
  {
    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern SafeFileHandle CreateFile(
      string fileName,
      [MarshalAs(UnmanagedType.U4)] uint fileAccess,
      [MarshalAs(UnmanagedType.U4)] uint fileShare,
      IntPtr securityAttributes,
      [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
      [MarshalAs(UnmanagedType.U4)] int flags,
      IntPtr template
    );

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(SafeFileHandle handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteConsoleOutputW(
      SafeFileHandle hConsoleOutput,
      CharInfo[] lpBuffer,
      Coord dwBufferSize,
      Coord dwBufferCoord,
      ref SmallRect lpWriteRegion);

    [StructLayout(LayoutKind.Sequential)]
    public record struct Coord(short X, short Y)
    {
      public short X = X;
      public short Y = Y;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CharUnion
    {
      [FieldOffset(0)] public ushort UnicodeChar;
      [FieldOffset(0)] public byte AsciiChar;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CharInfo
    {
      [FieldOffset(0)] public CharUnion Char;
      [FieldOffset(2)] public short Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
      public short Left;
      public short Top;
      public short Right;
      public short Bottom;
    }
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
