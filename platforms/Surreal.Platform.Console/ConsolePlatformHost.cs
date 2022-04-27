using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
using Surreal.Assets;
using Surreal.Diagnostics;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.IO;
using Surreal.Timing;

namespace Surreal;

/// <summary>A rune that can be painted to a <see cref="IConsoleGraphics"/>.</summary>
public readonly record struct Glyph(
  char Character,
  ConsoleColor ForegroundColor = ConsoleColor.White,
  ConsoleColor BackgroundColor = ConsoleColor.Black)
{
  public static implicit operator Glyph(char character) => new(character);
}

/// <summary>Allows managing the console display graphics.</summary>
public interface IConsoleGraphics
{
  int Width  { get; set; }
  int Height { get; set; }

  void Fill(Glyph glyph);
  void Draw(int x, int y, Glyph glyph);

  void Flush();
}

/// <summary>Allows accessing console platform internals.</summary>
public interface IConsolePlatformHost : IPlatformHost
{
  string  Title  { get; set; }
  new int Width  { get; set; }
  new int Height { get; set; }

  IConsoleGraphics Graphics { get; }
}

/// <summary>The <see cref="IPlatformHost"/> for <see cref="ConsolePlatform"/>.</summary>
[SupportedOSPlatform("windows")]
internal sealed class ConsolePlatformHost : IConsolePlatformHost, IConsoleGraphics
{
  private readonly ConsoleConfiguration configuration;
  private readonly SafeFileHandle consoleHandle;
  private readonly FrameCounter frameCounter = new();

  private Interop.CharInfo[] backBuffer = Array.Empty<Interop.CharInfo>();
  private IntervalTimer frameDisplayTimer = new(1.Seconds());

  public ConsolePlatformHost(ConsoleConfiguration configuration)
  {
    this.configuration = configuration;

    Keyboard = new ConsoleKeyboardDevice();
    Mouse    = new ConsoleMouseDevice();

    Title  = configuration.Title;
    Width  = configuration.Width;
    Height = configuration.Height;

    Mouse.IsCursorVisible = false; // disable by default

    var width = Math.Min(Console.LargestWindowWidth, configuration.Width);
    var height = Math.Min(Console.LargestWindowHeight, configuration.Height);

    Interop.SetCurrentFont(configuration.Font, configuration.FontSize);

    Console.SetWindowSize(width, height);
    Console.SetBufferSize(width, height);

    Console.CancelKeyPress += (_, _) => IsClosing = true;

    consoleHandle = Interop.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

    if (consoleHandle.IsInvalid)
    {
      throw new InvalidOperationException("Failed to acquire handle to attached console");
    }
  }

  public event Action<int, int>? Resized;

  private ConsoleKeyboardDevice Keyboard { get; }
  private ConsoleMouseDevice    Mouse    { get; }

  public string Title
  {
    get => Console.Title;
    set => Console.Title = value;
  }

  public int Width
  {
    get => Console.BufferWidth;
    set => Console.BufferWidth = value;
  }

  public int Height
  {
    get => Console.BufferHeight;
    set => Console.BufferHeight = value;
  }

  public bool IsVisible => true;
  public bool IsFocused => true;
  public bool IsClosing { get; private set; }

  public IConsoleGraphics Graphics => this;

  public void RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IPlatformHost>(this);
    services.AddSingleton<IConsolePlatformHost>(this);
    services.AddSingleton<IKeyboardDevice>(Keyboard);
    services.AddSingleton<IMouseDevice>(Mouse);
    services.AddSingleton(Graphics);
  }

  public void RegisterAssetLoaders(IAssetManager manager)
  {
    // no-op
  }

  public void RegisterFileSystems(IFileSystemRegistry registry)
  {
    // no-op
  }

  public void BeginFrame(TimeDelta deltaTime)
  {
    if (!IsClosing)
    {
      Keyboard.Update();
      Mouse.Update();

      // show the game's FPS in the window title
      if (configuration.ShowFpsInTitle)
      {
        frameCounter.Tick(deltaTime);

        if (frameDisplayTimer.Tick(deltaTime))
        {
          Title = $"{configuration.Title} - {frameCounter.TicksPerSecond:F} FPS";
        }
      }
    }
  }

  public void EndFrame(TimeDelta deltaTime)
  {
    if (!IsClosing)
    {
      Graphics.Flush();
    }
  }

  unsafe void IConsoleGraphics.Flush()
  {
    EnsureBackBufferSize();

    var rect = new Interop.SmallRect
    {
      Left   = 0,
      Top    = 0,
      Right  = (short) Width,
      Bottom = (short) Height
    };

    fixed (Interop.CharInfo* pointer = &backBuffer[0])
    {
      var result = Interop.WriteConsoleOutputW(
        hConsoleOutput: consoleHandle,
        lpBuffer: pointer,
        dwBufferSize: new((short) Width, (short) Height),
        dwBufferCoord: new(0, 0),
        lpWriteRegion: ref rect
      );

      if (!result)
      {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }
    }
  }

  void IConsoleGraphics.Fill(Glyph glyph)
  {
    EnsureBackBufferSize();

    Array.Fill(backBuffer, new()
    {
      Attributes = ToColorAttribute(glyph.ForegroundColor, glyph.BackgroundColor),
      Char = new()
      {
        UnicodeChar = glyph.Character
      },
    });
  }

  void IConsoleGraphics.Draw(int x, int y, Glyph glyph)
  {
    if (x < 0 || x > Width - 1) return;
    if (y < 0 || y > Height - 1) return;

    EnsureBackBufferSize();

    backBuffer[x + y * Width] = new()
    {
      Attributes = ToColorAttribute(glyph.ForegroundColor, glyph.BackgroundColor),
      Char = new()
      {
        UnicodeChar = glyph.Character
      },
    };
  }

  private void EnsureBackBufferSize()
  {
    if (backBuffer.Length != Width * Height)
    {
      Array.Resize(ref backBuffer, Width * Height);
    }
  }

  [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
  private static short ToColorAttribute(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
  {
    static int Cast(ConsoleColor color, bool isBackground)
    {
      var colorAttribute = (color & ~ConsoleColor.White) == ConsoleColor.Black ? (short) color : throw new InvalidOperationException();

      if (isBackground)
      {
        colorAttribute = (short) (colorAttribute << 4);
      }

      return colorAttribute;
    }

    return (short) (Cast(foregroundColor, false) | Cast(backgroundColor, true));
  }

  public void Dispose()
  {
    Interop.CloseHandle(consoleHandle);
  }

  /// <summary>A <see cref="IKeyboardDevice"/> for the Win32 console.</summary>
  private sealed class ConsoleKeyboardDevice : IKeyboardDevice
  {
    private readonly HashSet<Key> pressedLastFrame = new();
    private readonly HashSet<Key> pressedThisFrame = new();

    public event Action<Key>? KeyPressed;
    public event Action<Key>? KeyReleased;

    public bool IsKeyDown(Key key)
    {
      return Interop.GetAsyncKeyState(ScanCodes[key]) > 1;
    }

    public bool IsKeyUp(Key key)
    {
      return Interop.GetAsyncKeyState(ScanCodes[key]) == 0;
    }

    public bool IsKeyPressed(Key key)
    {
      return pressedThisFrame.Contains(key) && !pressedLastFrame.Contains(key);
    }

    public bool IsKeyReleased(Key key)
    {
      return pressedLastFrame.Contains(key) && !pressedThisFrame.Contains(key);
    }

    public void Update()
    {
      // clear the back buffer
      pressedLastFrame.Clear();

      foreach (var key in pressedThisFrame)
      {
        pressedLastFrame.Add(key);
      }

      pressedThisFrame.Clear();

      // update manually by checking each mapped scan code
      foreach (var key in ScanCodes.Keys)
      {
        if (IsKeyDown(key))
        {
          pressedThisFrame.Add(key);
        }
      }
    }

    /// <summary>A mapping of <see cref="Key"/> to Win32 scan codes.</summary>
    private static Dictionary<Key, int> ScanCodes { get; } = new()
    {
      [Key.Escape] = 0x1B,
      [Key.Space]  = 0x20,
      [Key.W]      = 0x57,
      [Key.S]      = 0x53,
      [Key.A]      = 0x41,
      [Key.D]      = 0x44,
    };
  }

  /// <summary>A <see cref="IMouseDevice"/> for the Win32 console.</summary>
  private sealed class ConsoleMouseDevice : IMouseDevice
  {
    public event Action<MouseButton>? ButtonPressed;
    public event Action<MouseButton>? ButtonReleased;
    public event Action<Vector2>?     Moved;

    private Vector2 lastPosition;

    public Vector2 Position
    {
      get
      {
        var (x, y) = Console.GetCursorPosition();

        return new Vector2(x, y);
      }
    }

    public Vector2 DeltaPosition { get; private set; }

    public bool IsCursorVisible
    {
      get => Console.CursorVisible;
      set => Console.CursorVisible = value;
    }

    public bool IsButtonDown(MouseButton button)
    {
      return false;
    }

    public bool IsButtonUp(MouseButton button)
    {
      return false;
    }

    public bool IsButtonPressed(MouseButton button)
    {
      return false;
    }

    public bool IsButtonReleased(MouseButton button)
    {
      return false;
    }

    public void Update()
    {
      DeltaPosition = Position - lastPosition;
      lastPosition  = Position;
    }
  }

  /// <summary>Native interop for Win32 consoles</summary>
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  private static class Interop
  {
    private const int FixedWidthTrueType = 54;
    private const int StandardOutputHandle = -11;

    [DllImport("user32.dll")]
    public static extern int GetAsyncKeyState(int vKeys);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool SetCurrentConsoleFontEx(
      IntPtr hConsoleOutput,
      bool MaximumWindow,
      ref FontInfo ConsoleCurrentFontEx
    );

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool GetCurrentConsoleFontEx(
      IntPtr hConsoleOutput,
      bool MaximumWindow,
      ref FontInfo ConsoleCurrentFontEx
    );

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

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern unsafe bool WriteConsoleOutputW(
      SafeFileHandle hConsoleOutput,
      CharInfo* lpBuffer,
      Coord dwBufferSize,
      Coord dwBufferCoord,
      ref SmallRect lpWriteRegion
    );

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(SafeFileHandle handle);

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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FontInfo
    {
      public int cbSize;
      public int FontIndex;
      public short FontWidth;
      public short FontSize;
      public int FontFamily;
      public int FontWeight;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string FontName;
    }

    public static void SetCurrentFont(string font, short fontSize = 0)
    {
      var handle = GetStdHandle(StandardOutputHandle);
      var fontInfo = new FontInfo
      {
        cbSize = Marshal.SizeOf<FontInfo>()
      };

      if (!GetCurrentConsoleFontEx(handle, false, ref fontInfo))
      {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }

      fontInfo.FontIndex  = 0;
      fontInfo.FontFamily = FixedWidthTrueType;
      fontInfo.FontName   = font;
      fontInfo.FontWeight = 400;
      fontInfo.FontSize   = fontSize > 0 ? fontSize : fontInfo.FontSize;

      // Get some settings from current font.
      if (!SetCurrentConsoleFontEx(handle, false, ref fontInfo))
      {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }
    }
  }
}
