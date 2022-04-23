using Surreal.Audio;
using Surreal.Diagnostics;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Internal;
using Surreal.Internal.Audio;
using Surreal.Internal.Graphics;
using Surreal.Internal.Input;
using Surreal.Timing;

namespace Surreal;

/// <summary>A specialization of <see cref="IPlatformHost"/> for desktop environments.</summary>
public interface IDesktopPlatformHost : IPlatformHost
{
  IDesktopWindow Window { get; }
}

/// <summary>Allows access to the platform's window.</summary>
public interface IDesktopWindow : IDisposable
{
  event Action<int, int> Resized;

  string Title  { get; set; }
  int    Width  { get; set; }
  int    Height { get; set; }

  bool IsVisible       { get; set; }
  bool IsFocused       { get; set; }
  bool IsVsyncEnabled  { get; set; }
  bool IsCursorVisible { get; set; }
  bool IsClosing       { get; }

  void Update();
  void Present();
}

internal sealed class DesktopPlatformHost : IDesktopPlatformHost, IServiceModule
{
  private readonly DesktopConfiguration configuration;

  private readonly FrameCounter frameCounter = new();
  private IntervalTimer frameDisplayTimer = new(1.Seconds());

  public DesktopPlatformHost(DesktopConfiguration configuration)
  {
    this.configuration = configuration;

    Window         = new OpenTKWindow(configuration);
    AudioServer    = new OpenTKAudioServer();
    GraphicsServer = new OpenTKGraphicsServer();
    InputServer    = new OpenTKInputServer(Window);
  }

  public event Action<int, int> Resized
  {
    add => Window.Resized += value;
    remove => Window.Resized -= value;
  }

  public OpenTKWindow         Window         { get; }
  public OpenTKAudioServer    AudioServer    { get; }
  public OpenTKGraphicsServer GraphicsServer { get; }
  public OpenTKInputServer    InputServer    { get; }

  public IServiceModule Services => this;

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
    services.AddSingleton<IDesktopPlatformHost>(this);
    services.AddSingleton<IDesktopWindow>(Window);
    services.AddSingleton<IAudioServer>(AudioServer);
    services.AddSingleton<IGraphicsServer>(GraphicsServer);
    services.AddSingleton<IInputServer>(InputServer);
    services.AddSingleton<IKeyboardDevice>(InputServer.Keyboard);
    services.AddSingleton<IMouseDevice>(InputServer.Mouse);
  }

  IDesktopWindow IDesktopPlatformHost.Window => Window;
}
