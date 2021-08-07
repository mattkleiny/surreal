using System;
using System.Diagnostics;
using System.Reflection;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Surreal.Platform.Internal
{
  internal sealed class OpenTKWindow : IDesktopWindow
  {
    private readonly GameWindow window;

    public OpenTKWindow(DesktopConfiguration configuration)
    {
      var gameWindowSettings = new GameWindowSettings
      {
        RenderFrequency = 0,
        UpdateFrequency = 0,
        IsMultiThreaded = false,
      };

      var nativeWindowSettings = new NativeWindowSettings
      {
        Title        = configuration.Title,
        Size         = new(configuration.Width, configuration.Height),
        WindowBorder = configuration.IsResizable ? WindowBorder.Resizable : WindowBorder.Fixed,
      };

      window = new GameWindow(gameWindowSettings, nativeWindowSettings)
      {
        VSync = configuration.IsVsyncEnabled ? VSyncMode.On : VSyncMode.Off,
      };

      try
      {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly != null)
        {
          // TODO: fix me
          // window.Icon = Icon.ExtractAssociatedIcon(assembly.Location);
        }
      }
      catch
      {
        // no-op
      }

      // TODO: set default width/height based on monitor resolution
      // TODO: center on-screen

      window.Resize += (_) => Resized?.Invoke(Width, Height);

      window.MakeCurrent();
    }

    public event Action<int, int>? Resized;

    public bool IsFocused => window.IsFocused;
    public bool IsClosing => window.IsExiting;

    public int Width
    {
      get => window.Size.X;
      set
      {
        Debug.Assert(value > 0, "value > 0");
        window.Size = new(value, window.Size.Y);
      }
    }

    public int Height
    {
      get => window.Size.Y;
      set
      {
        Debug.Assert(value > 0, "value > 0");
        window.Size = new(window.Size.X, value);
      }
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

    public string Title
    {
      get => window.Title;
      set => window.Title = value;
    }

    public bool IsVsyncEnabled
    {
      get => window.VSync != VSyncMode.Off;
      set => window.VSync = value ? VSyncMode.On : VSyncMode.Off;
    }

    public KeyboardState GetKeyboardState()
    {
      return window.KeyboardState;
    }

    public MouseState GetMouseState()
    {
      return window.MouseState;
    }

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
        window.SwapBuffers();
      }
    }

    public void Dispose()
    {
      window.Dispose();
    }
  }
}
