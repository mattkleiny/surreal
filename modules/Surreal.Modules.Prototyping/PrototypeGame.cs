using Surreal.Assets;
using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Diagnostics.Logging;
using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Images;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Textures;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Mathematics;
using Surreal.Screens;

namespace Surreal;

/// <summary>Base class for any <see cref="Game"/> that uses rapid prototyping services.</summary>
public abstract class PrototypeGame : Game
{
  private const LogLevel DefaultLogLevel = LogLevel.Trace;

  public IAudioDevice    AudioDevice    { get; private set; } = null!;
  public IComputeDevice  ComputeDevice  { get; private set; } = null!;
  public IGraphicsDevice GraphicsDevice { get; private set; } = null!;
  public IInputManager   InputManager   { get; private set; } = null!;
  public IKeyboardDevice Keyboard       { get; private set; } = null!;
  public IMouseDevice    Mouse          { get; private set; } = null!;
  public IScreenManager  Screens        { get; private set; } = null!;

  public Color ClearColor { get; set; } = Color.Black;

  protected override void Initialize()
  {
    LogFactory.Current = new CompositeLogFactory(
      new TextWriterLogFactory(Console.Out, DefaultLogLevel),
      new DebugLogFactory(DefaultLogLevel)
    );

    AudioDevice    = Host.Services.GetRequiredService<IAudioDevice>();
    ComputeDevice  = Host.Services.GetRequiredService<IComputeDevice>();
    GraphicsDevice = Host.Services.GetRequiredService<IGraphicsDevice>();
    Screens        = new ScreenManager(this);

    Plugins.Add(Screens);

    RegisterAssetLoaders(Assets);

    base.Initialize();

    OnResized(Host.Width, Host.Height); // initial resize
  }

  protected override void RegisterServices(IServiceRegistry services)
  {
    base.RegisterServices(services);

    InputManager = Host.Services.GetRequiredService<IInputManager>();
    Keyboard     = InputManager.GetRequiredDevice<IKeyboardDevice>();
    Mouse        = InputManager.GetRequiredDevice<IMouseDevice>();

    services.AddSingleton(AudioDevice);
    services.AddSingleton(GraphicsDevice);
    services.AddSingleton(InputManager);
    services.AddSingleton(Keyboard);
    services.AddSingleton(Mouse);
    services.AddSingleton(Screens);
  }

  protected virtual void RegisterAssetLoaders(IAssetManager assets)
  {
    assets.AddLoader(new AudioBufferLoader());
    assets.AddLoader(new AudioClipLoader(AudioDevice));
    assets.AddLoader(new BitmapFontLoader());
    assets.AddLoader(new ComputeProgramLoader(ComputeDevice));
    assets.AddLoader(new ImageLoader());
    assets.AddLoader(new MaterialLoader());
    assets.AddLoader(new ShaderProgramLoader(GraphicsDevice, hotReloading: Debugger.IsAttached));
    assets.AddLoader(new TextureLoader(GraphicsDevice, TextureFilterMode.Point, TextureWrapMode.Clamp));
    assets.AddLoader(new TextureRegionLoader());
    assets.AddLoader(new TrueTypeFontLoader());
  }

  protected override void OnResized(int width, int height)
  {
    base.OnResized(width, height);

    GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
  }

  protected override void Begin(GameTime time)
  {
    GraphicsDevice.BeginFrame();
    GraphicsDevice.Clear(ClearColor);

    base.Begin(time);
  }

  protected override void End(GameTime time)
  {
    base.End(time);

    GraphicsDevice.EndFrame();
    GraphicsDevice.Present();
  }
}
