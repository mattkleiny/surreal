using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Colors;
using Surreal.Diagnostics;
using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Images;
using Surreal.Graphics.Materials;
using Surreal.Input;
using Surreal.Internal;
using Surreal.Internal.Audio;
using Surreal.Internal.Graphics;
using Surreal.Internal.Input;
using Surreal.IO;
using Surreal.Resources;
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
  private readonly DesktopConfiguration _configuration;

  private readonly FrameCounter _frameCounter = new();
  private IntervalTimer _frameDisplayTimer = new(TimeSpan.FromSeconds(1));

  public DesktopPlatformHost(DesktopConfiguration configuration)
  {
    _configuration = configuration;

    Window = new SilkWindow(configuration);
    AudioBackend = new SilkAudioBackend();
    GraphicsBackend = new SilkGraphicsBackend();
    InputBackend = new SilkInputBackend();

    Resized += OnResized;
  }

  public SilkWindow Window { get; }
  public SilkAudioBackend AudioBackend { get; }
  public SilkGraphicsBackend GraphicsBackend { get; }
  public SilkInputBackend InputBackend { get; }

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
    services.AddService<IPlatformHost>(this);
    services.AddService<IDesktopPlatformHost>(this);
    services.AddService<IDesktopWindow>(Window);
    services.AddService<IAudioBackend>(AudioBackend);
    services.AddService<IGraphicsBackend>(GraphicsBackend);
    services.AddService<IInputBackend>(InputBackend);
    // services.AddService<IKeyboardDevice>(InputBackend.Keyboard);
    // services.AddService<IMouseDevice>(InputBackend.Mouse);
  }

  public void RegisterAssetLoaders(IResourceManager manager)
  {
    manager.AddLoader(new AudioBufferLoader());
    manager.AddLoader(new AudioClipLoader(AudioBackend));
    manager.AddLoader(new BitmapFontLoader());
    manager.AddLoader(new ColorPaletteLoader());
    manager.AddLoader(new ImageLoader());
    manager.AddLoader(new MaterialLoader());
  }

  public void RegisterFileSystems(IFileSystemRegistry registry)
  {
    // no-op
  }

  public void BeginFrame(TimeDelta deltaTime)
  {
    if (!IsClosing)
    {
      Window.Update();

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

  public void EndFrame(TimeDelta deltaTime)
  {
    if (!IsClosing)
    {
      Window.Present();
    }
  }

  public void Dispose()
  {
    Window.Dispose();
  }

  IDesktopWindow IDesktopPlatformHost.PrimaryWindow => Window;

  private void OnResized(int width, int height)
  {
    GraphicsBackend.SetViewportSize(new Viewport(0, 0, width, height));
  }
}
