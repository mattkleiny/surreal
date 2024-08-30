using Surreal.Audio;
using Surreal.Diagnostics;
using Surreal.Graphics;
using Surreal.Graphics.Images;
using Surreal.Input;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal;

/// <summary>
/// A specialization of <see cref="IPlatformHost" /> for desktop environments.
/// </summary>
public interface IDesktopPlatformHost : IPlatformHost
{
  IDesktopWindow PrimaryWindow { get; }
}

/// <summary>
/// Allows access to the platform's window.
/// </summary>
public interface IDesktopWindow : IDisposable
{
  event Action<int, int> Resized;

  string Title { get; set; }

  int Width { get; set; }
  int Height { get; set; }

  bool IsVisible { get; set; }
  bool IsVsyncEnabled { get; set; }
  bool IsEventDriven { get; set; }
  bool IsFocused { get; }
  bool IsClosing { get; }

  /// <summary>
  /// Sets the window icon to the given image.
  /// </summary>
  void SetWindowIcon(Image image);
}

internal sealed class DesktopPlatformHost : IDesktopPlatformHost
{
  private readonly FrameCounter _frameCounter = new();
  private readonly DesktopConfiguration _configuration;

  private IntervalTimer _frameDisplayTimer = new(TimeSpan.FromSeconds(1));

  public DesktopPlatformHost(DesktopConfiguration configuration)
  {
    _configuration = configuration;

    Window = new SilkWindow(configuration);
    AudioBackend = new SilkAudioBackend();
    GraphicsBackend = new SilkGraphicsBackend(configuration, Window.View);
    InputBackend = new SilkInputBackend(Window.View, Window.Input);

    Window.View.Update += OnUpdate;
    Window.View.Render += OnRender;
  }

  public SilkWindow Window { get; }
  public SilkAudioBackend AudioBackend { get; }
  public SilkGraphicsBackend GraphicsBackend { get; }
  public SilkInputBackend InputBackend { get; }

  IDesktopWindow IDesktopPlatformHost.PrimaryWindow => Window;

  public event Action<DeltaTime>? Update;
  public event Action<DeltaTime>? Render;

  public event Action<int, int> Resized
  {
    add => Window.Resized += value;
    remove => Window.Resized -= value;
  }

  public int Width => Window.Width;
  public int Height => Window.Height;

  public bool IsFocused => Window.IsFocused;
  public bool IsClosing => Window.IsClosing;
  public bool IsVisible => Window.IsVisible;

  public void RegisterServices(IServiceRegistry services)
  {
    services.AddService<IDesktopPlatformHost>(this);
    services.AddService<IDesktopWindow>(Window);
    services.AddService<IAudioBackend>(AudioBackend);
    services.AddService<IGraphicsBackend>(GraphicsBackend);
    services.AddService<IInputBackend>(InputBackend);
  }

  private void OnUpdate(double deltaTime)
  {
    if (!IsClosing && _configuration.ShowFpsInTitle)
    {
      _frameCounter.Tick(deltaTime);

      if (_frameDisplayTimer.Tick(deltaTime))
      {
        Window.Title = $"{_configuration.Title} - {_frameCounter.TicksPerSecond:F} FPS";
      }
    }

    Update?.Invoke(deltaTime);

    if (!IsClosing)
    {
      InputBackend.Update();
    }
  }

  private void OnRender(double deltaTime)
  {
    Render?.Invoke(deltaTime);
  }

  public Task RunAsync()
  {
    Window.Run();

    return Task.CompletedTask;
  }

  public void Close()
  {
    Window.Close();
  }

  public void Dispose()
  {
    Window.View.Update -= OnUpdate;
    Window.View.Render -= OnRender;

    Window.Dispose();
  }
}
