using Surreal.Audio;
using Surreal.Compute;
using Surreal.Diagnostics;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Internal;
using Surreal.Internal.Audio;
using Surreal.Internal.Compute;
using Surreal.Internal.Graphics;
using Surreal.Internal.Input;
using Surreal.Internal.Networking;
using Surreal.Networking;
using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

/// <summary>A specialization of <see cref="IPlatformHost"/> for desktop environments.</summary>
public interface IDesktopPlatformHost : IPlatformHost
{
  IDesktopWindow Window { get; }
}

internal sealed class DesktopPlatformHost : IDesktopPlatformHost, IServiceModule
{
  private readonly DesktopConfiguration configuration;

  private readonly FrameCounter  frameCounter      = new();
  private          IntervalTimer frameDisplayTimer = new(1.Seconds());

  public DesktopPlatformHost(DesktopConfiguration configuration)
  {
    this.configuration = configuration;

    Window         = new OpenTKWindow(configuration);
    AudioServer    = new OpenTKAudioServer();
    ComputeServer  = new OpenTKComputeServer();
    GraphicsServer = new OpenTKGraphicsServer();
    InputServer    = new OpenTKInputServer(Window);
    NetworkFactory = new DesktopNetworkFactory();
    Dispatcher     = new ImmediateDispatcher();
  }

  public event Action<int, int> Resized
  {
    add => Window.Resized += value;
    remove => Window.Resized -= value;
  }

  public OpenTKWindow          Window         { get; }
  public OpenTKAudioServer     AudioServer    { get; }
  public OpenTKComputeServer   ComputeServer  { get; }
  public OpenTKGraphicsServer  GraphicsServer { get; }
  public OpenTKInputServer     InputServer    { get; }
  public DesktopNetworkFactory NetworkFactory { get; }

  public IServiceModule Services   => this;
  public IDispatcher    Dispatcher { get; }

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
      Window.Present();

      InputServer.Update();

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

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IDesktopWindow>(Window);
    services.AddSingleton<IAudioServer>(AudioServer);
    services.AddSingleton<IComputeServer>(ComputeServer);
    services.AddSingleton<IGraphicsServer>(GraphicsServer);
    services.AddSingleton<IInputServer>(InputServer);
    services.AddSingleton<IKeyboardDevice>(InputServer.Keyboard);
    services.AddSingleton<IMouseDevice>(InputServer.Mouse);
    services.AddSingleton<INetworkFactory>(NetworkFactory);
  }

  IDesktopWindow IDesktopPlatformHost.Window => Window;
}
