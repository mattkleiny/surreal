using Surreal.Assets;
using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Images;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.IO.Xml;
using Surreal.Mathematics;
using Surreal.Screens;

namespace Surreal;

/// <summary>Base class for any <see cref="Game"/> that uses rapid prototyping services.</summary>
public abstract class PrototypeGame : Game
{
  public IAudioDevice    AudioDevice    => Services.GetRequiredService<IAudioDevice>();
  public IComputeDevice  ComputeDevice  => Services.GetRequiredService<IComputeDevice>();
  public IGraphicsDevice GraphicsDevice => Services.GetRequiredService<IGraphicsDevice>();
  public IKeyboardDevice Keyboard       => Services.GetRequiredService<IKeyboardDevice>();
  public IMouseDevice    Mouse          => Services.GetRequiredService<IMouseDevice>();
  public IScreenManager  Screens        => Services.GetRequiredService<IScreenManager>();

  public Color ClearColor { get; set; } = Color.Black;

  protected override void Initialize()
  {
    base.Initialize();

    OnResized(Host.Width, Host.Height); // initial resize
  }

  protected override void RegisterServices(IServiceRegistry services)
  {
    base.RegisterServices(services);

    services.AddSingleton<IScreenManager>(new ScreenManager(this));
    services.AddSingleton<IShaderParser>(new StandardShaderParser());
  }

  protected override void RegisterAssetLoaders(IAssetManager manager)
  {
    base.RegisterAssetLoaders(manager);

    manager.AddLoader(new AudioBufferLoader());
    manager.AddLoader(new AudioClipLoader(AudioDevice));
    manager.AddLoader(new BitmapFontLoader());
    manager.AddLoader(new ComputeProgramLoader(ComputeDevice));
    manager.AddLoader(new ImageLoader());
    manager.AddLoader(new MaterialLoader());
    manager.AddLoader(new ShaderProgramLoader(GraphicsDevice, Services.GetRequiredService<IShaderParser>(), hotReloading: Debugger.IsAttached));
    manager.AddLoader(new TextureLoader(GraphicsDevice, TextureFilterMode.Point, TextureWrapMode.Clamp, hotReloading: Debugger.IsAttached));
    manager.AddLoader(new TextureRegionLoader());
    manager.AddLoader(new TrueTypeFontLoader());
    manager.AddLoader(new XmlAssetLoader());
  }

  protected override void RegisterPlugins(IGamePluginRegistry plugins)
  {
    base.RegisterPlugins(plugins);

    plugins.Add(Services.GetRequiredService<IScreenManager>());
  }

  protected override void OnResized(int width, int height)
  {
    base.OnResized(width, height);

    GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
  }

  protected override void BeginFrame(GameTime time)
  {
    GraphicsDevice.BeginFrame();
    GraphicsDevice.Clear(ClearColor);

    base.BeginFrame(time);
  }

  protected override void EndFrame(GameTime time)
  {
    base.EndFrame(time);

    GraphicsDevice.EndFrame();
    GraphicsDevice.Present();
  }
}
