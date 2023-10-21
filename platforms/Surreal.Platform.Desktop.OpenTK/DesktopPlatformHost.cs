using Surreal.Audio;
using Surreal.Diagnostics;
using Surreal.Graphics;
using Surreal.Graphics.Images;
using Surreal.Input;
using Surreal.Timing;
using Surreal.Utilities;

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
  bool IsFocused { get; set; }
  bool IsVsyncEnabled { get; set; }
  bool IsCursorVisible { get; set; }
  bool IsEventDriven { get; set; }
  bool IsClosing { get; }

  /// <summary>
  /// Sets the window icon to the given image.
  /// </summary>
  void SetWindowIcon(Image image);
}

internal sealed class DesktopPlatformHost : IDesktopPlatformHost
{
  private readonly DesktopConfiguration _configuration;

  private readonly FrameCounter _frameCounter = new();
  private IntervalTimer _frameDisplayTimer = new(TimeSpan.FromSeconds(1));

  public DesktopPlatformHost(DesktopConfiguration configuration, IGameHost host)
  {
    _configuration = configuration;

    Window = new OpenTKWindow(configuration);
    AudioBackend = new OpenTKAudioBackend();
    GraphicsBackend = new OpenTKGraphicsBackend();
    InputBackend = new OpenTKInputBackend(Window);

    Resized += OnResized;

    host.Services.AddService<IPlatformHost>(this);
    host.Services.AddService<IDesktopPlatformHost>(this);
    host.Services.AddService<IDesktopWindow>(Window);
    host.Services.AddService<IAudioBackend>(AudioBackend);
    host.Services.AddService<IGraphicsBackend>(GraphicsBackend);
    host.Services.AddService<IInputBackend>(InputBackend);
  }

  public OpenTKWindow Window { get; }
  public OpenTKAudioBackend AudioBackend { get; }
  public OpenTKGraphicsBackend GraphicsBackend { get; }
  public OpenTKInputBackend InputBackend { get; }

  IDesktopWindow IDesktopPlatformHost.PrimaryWindow => Window;

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

  public void BeginFrame(DeltaTime deltaTime)
  {
    if (!IsClosing)
    {
      Window.Update();
      InputBackend.Update();

      // show the game's FPS in the window title
      if (_configuration.ShowFpsInTitle)
      {
        _frameCounter.Tick(deltaTime);

        if (_frameDisplayTimer.Tick(deltaTime))
        {
          Window.Title = $"{_configuration.Title} - {_frameCounter.TicksPerSecond:F} FPS";
        }
      }
    }
  }

  public void EndFrame(DeltaTime deltaTime)
  {
    if (!IsClosing)
    {
      Window.Present();
    }
  }

  public void Dispose()
  {
    AudioBackend.Dispose();
    Window.Dispose();
  }

  private void OnResized(int width, int height)
  {
    GraphicsBackend.SetViewportSize(new Viewport(0, 0, (uint)width, (uint)height));
  }
}
