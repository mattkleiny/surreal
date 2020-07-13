using System;
using System.Drawing;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics;

namespace Surreal.Platform.Internal {
  internal sealed class OpenTKWindow : IDisposable {
    private readonly GameWindow window;

    public OpenTKWindow(DesktopConfiguration configuration) {
      window = new GameWindow(
          width: configuration.Width   ?? 1024,
          height: configuration.Height ?? 768,
          mode: GraphicsMode.Default,
          title: configuration.Title,
          options: configuration.IsResizable ? GameWindowFlags.Default : GameWindowFlags.FixedWindow,
          device: DisplayDevice.Default,
          major: 2,
          minor: 0,
          flags: GraphicsContextFlags.Default) {
          VSync = configuration.IsVsyncEnabled ? VSyncMode.On : VSyncMode.Off
      };

      IsVisible = !configuration.WaitForFirstFrame;

      try {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly != null) {
          window.Icon = Icon.ExtractAssociatedIcon(assembly.Location);
        }
      }
      catch {
        // no-op
      }

      // set default width/height based on monitor resolution
      if (configuration.Width  == null) Width  = DisplayDevice.Default.Width  * 5 / 6;
      if (configuration.Height == null) Height = DisplayDevice.Default.Height * 5 / 6;

      // center on-screen
      window.X = DisplayDevice.Default.Width  / 2 - Width  / 2;
      window.Y = DisplayDevice.Default.Height / 2 - Height / 2;

      window.Resize += (sender, args) => Resized?.Invoke(Width, Height);

      window.MakeCurrent();
    }

    public event Action<int, int> Resized = null!;

    public bool IsFocused => window.Focused;
    public bool IsClosing => window.IsExiting;

    public int Width {
      get => window.Width;
      set {
        Check.That(value > 0, "value > 0");
        window.Width = value;
      }
    }

    public int Height {
      get => window.Height;
      set {
        Check.That(value > 0, "value > 0");
        window.Height = value;
      }
    }

    public bool IsVisible {
      get => window.Visible;
      set => window.Visible = value;
    }

    public bool IsCursorVisible {
      get => window.CursorVisible;
      set => window.CursorVisible = value;
    }

    public string Title {
      get => window.Title;
      set => window.Title = value;
    }

    public bool IsVsyncEnabled {
      get => window.VSync != VSyncMode.Off;
      set => window.VSync = value ? VSyncMode.On : VSyncMode.Off;
    }

    public void Update() {
      if (!IsClosing) {
        window.MakeCurrent();
        window.ProcessEvents();
      }
    }

    public void Present() {
      if (!IsClosing) {
        window.SwapBuffers();
      }
    }

    public void Dispose() {
      window.Dispose();
    }
  }
}