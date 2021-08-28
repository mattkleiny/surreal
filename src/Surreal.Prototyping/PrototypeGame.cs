using System.ComponentModel.Design;
using System.Diagnostics;
using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Content;
using Surreal.Diagnostics.Console;
using Surreal.Diagnostics.Logging;
using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Textures;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Mathematics;
using Surreal.Screens;
using Surreal.Services;

namespace Surreal
{
  public abstract class PrototypeGame : Game
  {
    public IAudioDevice    AudioDevice    { get; private set; } = null!;
    public IComputeDevice  ComputeDevice  { get; private set; } = null!;
    public IGraphicsDevice GraphicsDevice { get; private set; } = null!;
    public IInputManager   InputManager   { get; private set; } = null!;
    public IKeyboardDevice Keyboard       { get; private set; } = null!;
    public IMouseDevice    Mouse          { get; private set; } = null!;
    public IScreenManager  Screens        { get; private set; } = null!;
    public IGameConsole    Console        { get; private set; } = null!;

    public virtual LogLevel DefaultLogLevel => LogLevel.Trace;

    public Color ClearColor { get; set; } = Color.Black;

    protected override void Initialize()
    {
      Console = new GameConsole(new GameConsoleInterpreter(RegisterConsoleBindings));

      LogFactory.Current = new CompositeLogFactory(
          new ConsoleLogFactory(DefaultLogLevel),
          new DebugLogFactory(DefaultLogLevel),
          new GameConsoleLogFactory(Console, DefaultLogLevel)
      );

      AudioDevice    = Host.Services.GetRequiredService<IAudioDevice>();
      ComputeDevice  = Host.Services.GetRequiredService<IComputeDevice>();
      GraphicsDevice = Host.Services.GetRequiredService<IGraphicsDevice>();

      Plugins.Add(Screens = new ScreenManager(this));

      RegisterAssetLoaders(Assets);

      base.Initialize();

      OnResized(Host.Width, Host.Height); // initial resize
    }

    protected override void RegisterServices(IServiceContainer services)
    {
      base.RegisterServices(services);

      InputManager = Host.Services.GetRequiredService<IInputManager>();
      Keyboard     = InputManager.GetRequiredDevice<IKeyboardDevice>();
      Mouse        = InputManager.GetRequiredDevice<IMouseDevice>();

      services.AddService(AudioDevice);
      services.AddService(GraphicsDevice);
      services.AddService(InputManager);
      services.AddService(Keyboard);
      services.AddService(Mouse);
      services.AddService(Console);
      services.AddService(Screens);
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
      assets.AddLoader(new TrueTypeFontLoader());
    }

    protected virtual void RegisterConsoleBindings(IGameConsoleBindings bindings)
    {
      bindings.Add("exit", Exit);
      bindings.Add("clear", () => Console.Clear());
    }

    protected override void OnResized(int width, int height)
    {
      base.OnResized(width, height);

      GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
    }

    protected override void Begin(GameTime time)
    {
      GraphicsDevice.BeginFrame();

      if (ClearColor != Color.Clear)
      {
        GraphicsDevice.Clear(ClearColor);
      }

      base.Begin(time);
    }

    protected override void End(GameTime time)
    {
      base.End(time);

      GraphicsDevice.EndFrame();
      GraphicsDevice.Present();
    }
  }
}
