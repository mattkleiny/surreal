using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

/// <summary>The <see cref="IPlatformHost"/> for <see cref="ConsolePlatform"/>.</summary>
internal sealed class ConsolePlatformHost : IPlatformHost, IServiceModule
{
  private readonly ConsoleConfiguration configuration;

  public ConsolePlatformHost(ConsoleConfiguration configuration)
  {
    this.configuration = configuration;

    Keyboard = new ConsoleKeyboardDevice();
    Mouse = new ConsoleMouseDevice();
    Dispatcher = new ImmediateDispatcher();
  }

  public event Action<int, int>? Resized;

  private ConsoleKeyboardDevice Keyboard { get; }
  private ConsoleMouseDevice    Mouse    { get; }

  public int  Width     => Console.BufferWidth;
  public int  Height    => Console.BufferHeight;
  public bool IsVisible => true;
  public bool IsFocused => true;
  public bool IsClosing => false;

  public IServiceModule Services   => this;
  public IDispatcher    Dispatcher { get; }

  public void Tick(DeltaTime deltaTime)
  {
    Keyboard.Update();
    Mouse.Update();
  }

  public void Dispose()
  {
  }

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IKeyboardDevice>(Keyboard);
    services.AddSingleton<IMouseDevice>(Mouse);
  }

  private sealed class ConsoleKeyboardDevice : IKeyboardDevice
  {
    public event Action<Key>? KeyPressed;
    public event Action<Key>? KeyReleased;

    public bool IsKeyDown(Key key)
    {
      return false;
    }

    public bool IsKeyUp(Key key)
    {
      return false;
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
  }

  private sealed class ConsoleMouseDevice : IMouseDevice
  {
    public event Action<MouseButton>? ButtonPressed;
    public event Action<MouseButton>? ButtonReleased;
    public event Action<Vector2>?     Moved;

    public Vector2 Position      { get; private set; }
    public Vector2 DeltaPosition { get; private set; }

    public bool IsCursorVisible { get; set; }

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

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
  private static class Interop
  {
    public const nint INVALID_HANDLE_VALUE = -1;
    public const nint STD_INPUT_HANDLE = -10;
    public const nint STD_OUTPUT_HANDLE = -11;
    public const nint STD_ERROR_HANDLE = -12;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(nint nStdHandle);

    // http://pinvoke.net/default.aspx/kernel32/ReadConsoleInput.html
    [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
    public static extern bool ReadConsoleInput(
      IntPtr hConsoleInput,
      [Out] INPUT_RECORD[] lpBuffer,
      uint nLength,
      out uint lpNumberOfEventsRead
    );

    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
      public short X;
      public short Y;
    }

    public enum InputEventTypes : ushort
    {
      KEY_EVENT = 0x1,
      MOUSE_EVENT = 0x2,
      WINDOW_BUFFER_SIZE_EVENT = 0x4,
      MENU_EVENT = 0x8,
      FOCUS_EVENT = 0x10,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT_RECORD
    {
      [FieldOffset(0)]
      public ushort EventType;
      [FieldOffset(4)]
      public KEY_EVENT_RECORD KeyEvent;
      [FieldOffset(4)]
      public MOUSE_EVENT_RECORD MouseEvent;
      [FieldOffset(4)]
      public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
      [FieldOffset(4)]
      public MENU_EVENT_RECORD MenuEvent;
      [FieldOffset(4)]
      public FOCUS_EVENT_RECORD FocusEvent;
    };

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct KEY_EVENT_RECORD
    {
      [FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
      public bool bKeyDown;
      [FieldOffset(4), MarshalAs(UnmanagedType.U2)]
      public ushort wRepeatCount;
      [FieldOffset(6), MarshalAs(UnmanagedType.U2)]
      public ushort wVirtualKeyCode;
      [FieldOffset(8), MarshalAs(UnmanagedType.U2)]
      public ushort wVirtualScanCode;
      [FieldOffset(10)]
      public char UnicodeChar;
      [FieldOffset(12), MarshalAs(UnmanagedType.U4)]
      public uint dwControlKeyState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSE_EVENT_RECORD
    {
      public COORD dwMousePosition;
      public uint dwButtonState;
      public uint dwControlKeyState;
      public uint dwEventFlags;
    }

    public struct WINDOW_BUFFER_SIZE_RECORD
    {
      public COORD dwSize;

      public WINDOW_BUFFER_SIZE_RECORD(short x, short y)
      {
        dwSize = new COORD
        {
          X = x,
          Y = y,
        };
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MENU_EVENT_RECORD
    {
      public uint dwCommandId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FOCUS_EVENT_RECORD
    {
      public uint bSetFocus;
    }
  }
}
