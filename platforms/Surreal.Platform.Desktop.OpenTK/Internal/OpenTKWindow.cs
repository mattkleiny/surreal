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
  private readonly DesktopConfiguration _configuration;
  private readonly GameWindow _window;
  private bool _hasPassedFirstFrame;

  public OpenTKWindow(DesktopConfiguration configuration)
  {
    _configuration = configuration;
    var gameWindowSettings = new GameWindowSettings
    {
      RenderFrequency = 0,
      UpdateFrequency = 0,
      IsMultiThreaded = false
    };

    var nativeWindowSettings = new NativeWindowSettings
    {
      APIVersion = configuration.OpenGlVersion,
      Title = configuration.Title,
      WindowBorder = configuration.IsResizable ? WindowBorder.Resizable : WindowBorder.Fixed,
      StartVisible = !configuration.WaitForFirstFrame,
      IsEventDriven = configuration.IsEventDriven,
      StartFocused = true,
      Size = new Vector2i(configuration.Width, configuration.Height),
      Flags = GetContextFlags(),
      Profile = ContextProfile.Core,
      NumberOfSamples = 0
    };

    _window = new GameWindow(gameWindowSettings, nativeWindowSettings)
    {
      VSync = configuration.IsVsyncEnabled ? VSyncMode.On : VSyncMode.Off,
      IsVisible = !configuration.WaitForFirstFrame
    };

    if (configuration.IconPath.HasValue)
    {
      using var image = Image.Load(configuration.IconPath.Value);

      SetWindowIcon(image);
    }

    _window.Resize += _ => Resized?.Invoke(Width, Height);
    _window.FocusedChanged += OnFocusChanged;

    _window.MakeCurrent();
  }

  public KeyboardState KeyboardState => _window.KeyboardState;
  public MouseState MouseState => _window.MouseState;

  public event Action<int, int>? Resized;

  public int Width
  {
    get => _window.Size.X;
    set
    {
      Debug.Assert(value > 0, "value > 0");
      _window.Size = new Vector2i(value, _window.Size.Y);
    }
  }

  public int Height
  {
    get => _window.Size.Y;
    set
    {
      Debug.Assert(value > 0, "value > 0");
      _window.Size = new Vector2i(_window.Size.X, value);
    }
  }

  public string Title
  {
    get => _window.Title;
    set => _window.Title = value;
  }

  public bool IsVisible
  {
    get => _window.IsVisible;
    set => _window.IsVisible = value;
  }

  public bool IsCursorVisible
  {
    get => _window.CursorVisible;
    set => _window.CursorVisible = value;
  }

  public bool IsEventDriven
  {
    get => _window.IsEventDriven;
    set => _window.IsEventDriven = value;
  }

  public bool IsVsyncEnabled
  {
    get => _window.VSync != VSyncMode.Off;
    set => _window.VSync = value ? VSyncMode.On : VSyncMode.Off;
  }

  public bool IsFocused
  {
    get => _window.IsFocused;
    set
    {
      if (value)
      {
        _window.Focus();
      }
    }
  }

  public bool IsClosing => _window.IsExiting;

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

      GLFW.SetWindowIcon(_window.WindowPtr, images);
    }
  }

  public void Dispose()
  {
    _window.Dispose();
  }

  public void Update()
  {
    if (!IsClosing)
    {
      _window.ProcessEvents();
    }
  }

  public void Present()
  {
    if (!IsClosing)
    {
      if (!_hasPassedFirstFrame && _configuration.WaitForFirstFrame)
      {
        _window.IsVisible = true;
        _hasPassedFirstFrame = true;
      }

      _window.SwapBuffers();
    }
  }

  private void OnFocusChanged(FocusedChangedEventArgs eventArgs)
  {
    if (!_configuration.RunInBackground)
    {
      if (!eventArgs.IsFocused && _configuration.ShowFpsInTitle)
      {
        _window.Title = _configuration.Title;
      }

      _window.IsEventDriven = !eventArgs.IsFocused;
    }
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


