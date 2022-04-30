using Surreal.Assets;
using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Diagnostics;
using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Images;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Input;
using Surreal.Internal;
using Surreal.Internal.Audio;
using Surreal.Internal.Graphics;
using Surreal.Internal.Input;
using Surreal.IO;
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
}

internal sealed class DesktopPlatformHost : IDesktopPlatformHost
{
  private readonly DesktopConfiguration configuration;

  private readonly FrameCounter frameCounter = new();
  private IntervalTimer frameDisplayTimer = new(1.Seconds());

  public DesktopPlatformHost(DesktopConfiguration configuration)
  {
    this.configuration = configuration;

    Window         = new OpenTKWindow(configuration);
    AudioServer    = new OpenTKAudioServer();
    GraphicsServer = new OpenTKGraphicsServer(configuration.OpenGlVersion);
    InputServer    = new OpenTKInputServer(Window);

    Resized += OnResized;
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

  public int Width  => Window.Width;
  public int Height => Window.Height;

  public bool IsFocused => Window.IsFocused;
  public bool IsClosing => Window.IsClosing;
  public bool IsVisible => Window.IsVisible;

  public void RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IPlatformHost>(this);
    services.AddSingleton<IDesktopPlatformHost>(this);
    services.AddSingleton<IDesktopWindow>(Window);
    services.AddSingleton<IAudioServer>(AudioServer);
    services.AddSingleton<IGraphicsServer>(GraphicsServer);
    services.AddSingleton<IInputServer>(InputServer);
  }

  public void RegisterAssetLoaders(IAssetManager manager)
  {
    manager.AddLoader(new AudioBufferLoader());
    manager.AddLoader(new AudioClipLoader(AudioServer));
    manager.AddLoader(new BitmapFontLoader());
    manager.AddLoader(new TrueTypeFontLoader(GraphicsServer));
    manager.AddLoader(new ColorPaletteLoader());
    manager.AddLoader(new ImageLoader());
    manager.AddLoader(new ShaderProgramLoader(GraphicsServer));
    manager.AddLoader(new OpenTKShaderProgramLoader(GraphicsServer));
    manager.AddLoader(new ShaderDeclarationLoader());
    manager.AddLoader(new TextureLoader(GraphicsServer));
    manager.AddLoader(new TextureRegionLoader());
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
      InputServer.Update();

      // show the game's FPS in the window title
      if (configuration.ShowFpsInTitle)
      {
        frameCounter.Tick(deltaTime);

        if (frameDisplayTimer.Tick(deltaTime))
        {
          Window.Title = $"{configuration.Title} - {frameCounter.TicksPerSecond:F} FPS";
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
    AudioServer.Dispose();
    Window.Dispose();
  }

  private void OnResized(int width, int height)
  {
    GraphicsServer.SetViewportSize(new Viewport(0, 0, width, height));
  }

  IDesktopWindow IDesktopPlatformHost.Window => Window;
}
