using System;
using Surreal.Audio.SPI;
using Surreal.Compute.SPI;
using Surreal.Diagnostics;
using Surreal.Graphics.SPI;
using Surreal.Input;
using Surreal.IO;
using Surreal.Platform.Internal;
using Surreal.Platform.Internal.Audio;
using Surreal.Platform.Internal.Compute;
using Surreal.Platform.Internal.Graphics;
using Surreal.Platform.Internal.Input;
using Surreal.Timing;

namespace Surreal.Platform {
  internal sealed class DesktopPlatformHost : IDesktopPlatformHost, IServiceProvider {
    private readonly OpenTKWindow         window;
    private readonly DesktopConfiguration configuration;

    private readonly FrameCounter  frameCounter      = new FrameCounter();
    private readonly IntervalTimer frameDisplayTimer = new IntervalTimer(1.Seconds());

    public DesktopPlatformHost(OpenTKWindow window, DesktopConfiguration configuration) {
      this.window        = window;
      this.configuration = configuration;

      AudioBackend    = new OpenTKAudioBackend();
      ComputeBackend  = new OpenTKComputeBackend();
      GraphicsBackend = new OpenTKGraphicsBackend(window);
      InputManager    = new OpenTKInputManager(window);
      FileSystem      = new LocalFileSystem();
    }

    public OpenTKAudioBackend    AudioBackend    { get; }
    public OpenTKComputeBackend  ComputeBackend  { get; }
    public OpenTKGraphicsBackend GraphicsBackend { get; }
    public OpenTKInputManager    InputManager    { get; }
    public LocalFileSystem       FileSystem      { get; }

    public event Action<int, int> Resized {
      add => window.Resized += value;
      remove => window.Resized -= value;
    }

    public IServiceProvider Services => this;

    public bool IsFocused => window.IsFocused;
    public bool IsClosing => window.IsClosing;

    public int Width {
      get => window.Width;
      set => window.Width = value;
    }

    public int Height {
      get => window.Height;
      set => window.Height = value;
    }

    public bool IsVisible {
      get => window.IsVisible;
      set => window.IsVisible = value;
    }

    public string Title {
      get => window.Title;
      set => window.Title = value;
    }

    public bool IsVsyncEnabled {
      get => window.IsVsyncEnabled;
      set => window.IsVsyncEnabled = value;
    }

    public void Tick(DeltaTime deltaTime) {
      if (!IsClosing) {
        window.Update();

        InputManager.Update();

        // show the window after the first frame is complete
        if (configuration.WaitForFirstFrame && !window.IsVisible) {
          window.IsVisible = true;
        }

        // show the game's FPS in the window title
        if (configuration.ShowFPSInTitle) {
          frameCounter.Tick(deltaTime);

          if (frameDisplayTimer.Tick()) {
            Title = $"{configuration.Title} - {frameCounter.FramesPerSecond:F} FPS";
          }
        }
      }
    }

    object? IServiceProvider.GetService(Type serviceType) {
      if (serviceType == typeof(IAudioBackend)) return AudioBackend;
      if (serviceType == typeof(IComputeBackend)) return ComputeBackend;
      if (serviceType == typeof(IGraphicsBackend)) return GraphicsBackend;
      if (serviceType == typeof(IInputManager)) return InputManager;
      if (serviceType == typeof(IFileSystem)) return FileSystem;

      return null;
    }

    public void Dispose() {
      GraphicsBackend.Dispose();
      ComputeBackend.Dispose();
      AudioBackend.Dispose();

      window.Dispose();
    }
  }
}