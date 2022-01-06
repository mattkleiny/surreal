using Surreal.Audio;
using Surreal.Compute;
using Surreal.Diagnostics;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Internal;
using Surreal.Internal.Audio;
using Surreal.Internal.Compute;
using Surreal.Internal.Graphics;
using Surreal.Internal.Input;
using Surreal.Timing;

namespace Surreal;

internal sealed class DesktopPlatformHost : IDesktopPlatformHost, IServiceProvider
{
  private readonly DesktopConfiguration configuration;

  private readonly FrameCounter  frameCounter      = new();
  private          IntervalTimer frameDisplayTimer = new(1.Seconds());

  public DesktopPlatformHost(DesktopConfiguration configuration)
  {
    this.configuration = configuration;

    Window         = new OpenTkWindow(configuration);
    AudioDevice    = new OpenTkAudioDevice();
    ComputeDevice  = new OpenTkComputeDevice();
    GraphicsDevice = new OpenTkGraphicsDevice(Window);
    InputManager   = new OpenTkInputManager(Window);
  }

  public event Action<int, int> Resized
  {
    add => Window.Resized += value;
    remove => Window.Resized -= value;
  }

  public OpenTkWindow         Window         { get; }
  public OpenTkAudioDevice    AudioDevice    { get; }
  public OpenTkComputeDevice  ComputeDevice  { get; }
  public OpenTkGraphicsDevice GraphicsDevice { get; }
  public OpenTkInputManager   InputManager   { get; }

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

      // show the game's FPS in the window title
      if (configuration.ShowFpsInTitle)
      {
        frameCounter.Tick(deltaTime);

        if (frameDisplayTimer.Tick(deltaTime))
        {
          Window.Title = $"{configuration.Title} - {frameCounter.FramesPerSecond:F} FPS";
        }
      }
    }
  }

  public void Dispose()
  {
    Window.Dispose();
  }

  object? IServiceProvider.GetService(Type serviceType)
  {
    if (serviceType == typeof(IDesktopWindow)) return Window;
    if (serviceType == typeof(IAudioDevice)) return AudioDevice;
    if (serviceType == typeof(IComputeDevice)) return ComputeDevice;
    if (serviceType == typeof(IGraphicsDevice)) return GraphicsDevice;
    if (serviceType == typeof(IInputManager)) return InputManager;

    return null;
  }

  IDesktopWindow IDesktopPlatformHost.Window => Window;
}
