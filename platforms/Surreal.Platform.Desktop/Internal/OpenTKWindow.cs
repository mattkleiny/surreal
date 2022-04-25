using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Surreal.Internal;

internal sealed class OpenTKWindow : IDesktopWindow
{
  private readonly DesktopConfiguration configuration;
  private readonly GameWindow window;
  private bool hasPassedFirstFrame;

  public OpenTKWindow(DesktopConfiguration configuration)
  {
    this.configuration = configuration;
    var gameWindowSettings = new GameWindowSettings
    {
      RenderFrequency = 0,
      UpdateFrequency = 0,
      IsMultiThreaded = false,
    };

    var nativeWindowSettings = new NativeWindowSettings
    {
      Title           = configuration.Title,
      WindowBorder    = configuration.IsResizable ? WindowBorder.Resizable : WindowBorder.Fixed,
      StartVisible    = !configuration.WaitForFirstFrame,
      Size            = new Vector2i(configuration.Width, configuration.Height),
      Flags           = ContextFlags.ForwardCompatible | ContextFlags.Debug,
      Profile         = ContextProfile.Core,
      NumberOfSamples = 0,
      APIVersion      = configuration.OpenGlVersion,
    };

    window = new GameWindow(gameWindowSettings, nativeWindowSettings)
    {
      VSync     = configuration.IsVsyncEnabled ? VSyncMode.On : VSyncMode.Off,
      IsVisible = !configuration.WaitForFirstFrame,
    };

    window.Resize += _ => Resized?.Invoke(Width, Height);

    window.MakeCurrent();
  }

  public event Action<int, int>? Resized;

  public int Width
  {
    get => window.Size.X;
    set
    {
      Debug.Assert(value > 0, "value > 0");
      window.Size = new Vector2i(value, window.Size.Y);
    }
  }

  public int Height
  {
    get => window.Size.Y;
    set
    {
      Debug.Assert(value > 0, "value > 0");
      window.Size = new Vector2i(window.Size.X, value);
    }
  }

  public string Title
  {
    get => window.Title;
    set => window.Title = value;
  }

  public bool IsVisible
  {
    get => window.IsVisible;
    set => window.IsVisible = value;
  }

  public bool IsCursorVisible
  {
    get => window.CursorVisible;
    set => window.CursorVisible = value;
  }

  public bool IsVsyncEnabled
  {
    get => window.VSync != VSyncMode.Off;
    set => window.VSync = value ? VSyncMode.On : VSyncMode.Off;
  }

  public bool IsFocused
  {
    get => window.IsFocused;
    set
    {
      if (value)
      {
        window.Focus();
      }
    }
  }

  public bool IsClosing => window.IsExiting;

  public KeyboardState KeyboardState => window.KeyboardState;
  public MouseState    MouseState    => window.MouseState;

  public void Update()
  {
    if (!IsClosing)
    {
      window.MakeCurrent();
      window.ProcessEvents();
    }
  }

  public void Present()
  {
    if (!IsClosing)
    {
      if (!hasPassedFirstFrame && configuration.WaitForFirstFrame)
      {
        window.IsVisible    = true;
        hasPassedFirstFrame = true;
      }

      window.SwapBuffers();
    }
  }

  public void Dispose()
  {
    window.Dispose();
  }
}
