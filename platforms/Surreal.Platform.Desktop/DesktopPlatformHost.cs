using System;
using Surreal.Audio;
using Surreal.Compute;
using Surreal.Diagnostics;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.IO;
using Surreal.Platform.Internal.Audio;
using Surreal.Platform.Internal.Compute;
using Surreal.Platform.Internal.Graphics;
using Surreal.Platform.Internal.Input;
using Surreal.Timing;

namespace Surreal.Platform
{
  internal sealed class DesktopPlatformHost : IDesktopPlatformHost, IServiceProvider
  {
    private readonly DesktopConfiguration configuration;

    private readonly FpsCounter fpsCounter        = new();
    private          Timer      frameDisplayTimer = new(1.Seconds());

    public DesktopPlatformHost(IDesktopWindow window, DesktopConfiguration configuration)
    {
      this.configuration = configuration;

      Window         = window;
      AudioDevice    = new OpenTKAudioDevice();
      ComputeDevice  = new OpenTKComputeDevice();
      GraphicsDevice = new OpenTKGraphicsDevice(window);
      InputManager   = new OpenTKInputManager(window);
      FileSystem     = new LocalFileSystem();
    }

    public IDesktopWindow       Window         { get; }
    public OpenTKAudioDevice    AudioDevice    { get; }
    public OpenTKComputeDevice  ComputeDevice  { get; }
    public OpenTKGraphicsDevice GraphicsDevice { get; }
    public OpenTKInputManager   InputManager   { get; }
    public LocalFileSystem      FileSystem     { get; }

    public event Action<int, int> Resized
    {
      add => Window.Resized += value;
      remove => Window.Resized -= value;
    }

    public IServiceProvider Services => this;

    public int Width  => Window.Width;
    public int Height => Window.Height;

    public bool IsFocused => Window.IsFocused;
    public bool IsClosing => Window.IsClosing;
    public bool IsVisible => Window.IsVisible;

    public void Tick(DeltaTime deltaTime)
    {
      if (!IsClosing)
      {
        Window.Update();
        InputManager.Update();

        // show the window after the first frame is complete
        if (configuration.WaitForFirstFrame && !Window.IsVisible)
        {
          Window.IsVisible = true;
        }

        // show the game's FPS in the window title
        if (configuration.ShowFPSInTitle)
        {
          fpsCounter.Tick(deltaTime);

          if (frameDisplayTimer.Tick(deltaTime))
          {
            Window.Title = $"{configuration.Title} - {fpsCounter.FramesPerSecond.ToString("F")} FPS";
          }
        }
      }
    }

    object? IServiceProvider.GetService(Type serviceType)
    {
      if (serviceType == typeof(IDesktopWindow)) return Window;
      if (serviceType == typeof(IAudioDevice)) return AudioDevice;
      if (serviceType == typeof(IComputeDevice)) return ComputeDevice;
      if (serviceType == typeof(IGraphicsDevice)) return GraphicsDevice;
      if (serviceType == typeof(IInputManager)) return InputManager;
      if (serviceType == typeof(IFileSystem)) return FileSystem;

      return null;
    }

    public void Dispose()
    {
      GraphicsDevice.Dispose();
      ComputeDevice.Dispose();
      AudioDevice.Dispose();

      Window.Dispose();
    }
  }
}