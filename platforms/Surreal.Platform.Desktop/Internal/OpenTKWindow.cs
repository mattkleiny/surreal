using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Image = Surreal.Graphics.Images.Image;
using RawImage = OpenTK.Windowing.GraphicsLibraryFramework.Image;

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
      IsMultiThreaded = false
    };

    var nativeWindowSettings = new NativeWindowSettings
    {
      APIVersion      = configuration.OpenGlVersion,
      Title           = configuration.Title,
      WindowBorder    = configuration.IsResizable ? WindowBorder.Resizable : WindowBorder.Fixed,
      StartVisible    = !configuration.WaitForFirstFrame,
      IsEventDriven   = configuration.IsEventDriven,
      StartFocused    = true,
      Size            = new Vector2i(configuration.Width, configuration.Height),
      Flags           = GetContextFlags(),
      Profile         = ContextProfile.Core,
      NumberOfSamples = 0
    };

    window = new GameWindow(gameWindowSettings, nativeWindowSettings)
    {
      VSync     = configuration.IsVsyncEnabled ? VSyncMode.On : VSyncMode.Off,
      IsVisible = !configuration.WaitForFirstFrame
    };

    if (configuration.IconPath.HasValue)
    {
      using var image = Image.Load(configuration.IconPath.Value);

      SetWindowIcon(image);
    }

    window.Resize         += _ => Resized?.Invoke(Width, Height);
    window.FocusedChanged += OnFocusChanged;

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

  public unsafe void SetWindowIcon(Image image)
  {
    // convert our Color32 to raw bytes, pass a pointer down to GLFW
    var rgba = MemoryMarshal.AsBytes(image.Pixels.ToReadOnlySpan());

    fixed (byte* pixels = rgba)
    {
      ReadOnlySpan<RawImage> images = stackalloc RawImage[]
      {
        new RawImage(image.Width, image.Height, pixels)
      };

      GLFW.SetWindowIcon(window.WindowPtr, images);
    }
  }

  private void OnFocusChanged(FocusedChangedEventArgs eventArgs)
  {
    if (!configuration.RunInBackground)
    {
      if (!eventArgs.IsFocused && configuration.ShowFpsInTitle)
      {
        window.Title = configuration.Title;
      }

      window.IsEventDriven = !eventArgs.IsFocused;
    }
  }

  public void Dispose()
  {
    window.Dispose();
  }

  private static ContextFlags GetContextFlags()
  {
    var flags = ContextFlags.ForwardCompatible;

#if DEBUG
    flags |= ContextFlags.Debug;
#endif

    return flags;
  }
}
