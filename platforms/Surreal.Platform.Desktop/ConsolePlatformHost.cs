using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
using Surreal.Diagnostics;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

/// <summary>Allows accessing console platform internals.</summary>
public interface IConsolePlatformHost : IPlatformHost
{
  string Title  { get; set; }
  int    Width  { get; set; }
  int    Height { get; set; }
}

/// <summary>The <see cref="IPlatformHost"/> for <see cref="ConsolePlatform"/>.</summary>
[SupportedOSPlatform("windows")]
internal sealed class ConsolePlatformHost : IConsolePlatformHost, IServiceModule
{
  private readonly ConsoleConfiguration configuration;

  private readonly FrameCounter frameCounter = new();
  private IntervalTimer frameDisplayTimer = new(1.Seconds());

  public ConsolePlatformHost(ConsoleConfiguration configuration)
  {
    this.configuration = configuration;

    Keyboard   = new ConsoleKeyboardDevice();
    Mouse      = new ConsoleMouseDevice();
    Dispatcher = new ImmediateDispatcher();

    Title  = configuration.Title;
    Width  = configuration.Width;
    Height = configuration.Height;

    Mouse.IsCursorVisible = false; // disable by default

    Interop.SetCurrentFont(configuration.Font, configuration.FontSize);

    Console.SetWindowSize(configuration.Width + 1, configuration.Height + 2);
    Console.SetBufferSize(configuration.Width + 1, configuration.Height + 2);

    Console.CancelKeyPress += (_, _) => IsClosing = true;
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

  public IServiceModule Services   => this;
  public IDispatcher    Dispatcher { get; }

  public void Tick(DeltaTime deltaTime)
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
          Title = $"{configuration.Title} - {frameCounter.FramesPerSecond:F} FPS";
        }
      }
    }
  }

  public void Dispose()
  {
  }

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IConsolePlatformHost>(this);
    services.AddSingleton<IKeyboardDevice>(Keyboard);
    services.AddSingleton<IMouseDevice>(Mouse);
  }

  /// <summary>A <see cref="IKeyboardDevice"/> for the Win32 console.</summary>
  private sealed class ConsoleKeyboardDevice : IKeyboardDevice
  {
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
      return false;
    }

    public bool IsKeyReleased(Key key)
    {
      return false;
    }

    public void Update()
    {
    }

    /// <summary>A mapping of <see cref="Key"/> to Win32 scan codes.</summary>
    private static Dictionary<Key, int> ScanCodes { get; } = new()
    {
      [Key.Escape] = 0x1B,
      [Key.Space]  = 0x20,
    };
  }

  /// <summary>A <see cref="IMouseDevice"/> for the Win32 console.</summary>
  private sealed class ConsoleMouseDevice : IMouseDevice
  {
    public event Action<MouseButton>? ButtonPressed;
    public event Action<MouseButton>? ButtonReleased;
    public event Action<Vector2>?     Moved;

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
    }
  }

  /// <summary>Native interop for Win32 consoles</summary>
  private static class Interop
  {
    private const int FixedWidthTrueType = 54;
    private const int StandardOutputHandle = -11;

    [DllImport("user32.dll")]
    public static extern int GetAsyncKeyState(int vKeys);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

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
    public static extern bool WriteConsoleOutputW(
      SafeFileHandle hConsoleOutput,
      CharInfo[] lpBuffer,
      Coord dwBufferSize,
      Coord dwBufferCoord,
      ref SmallRect lpWriteRegion);

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
