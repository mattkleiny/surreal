using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Diagnostics.Console;
using Surreal.Diagnostics.Console.Interpreter;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Materials.Shaders;
using Surreal.Graphics.Materials.Shaders.Spirv;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Text;
using Surreal.Utilities;

namespace Surreal {
  public abstract class GameJam : Game {
    public new static GameJam Current => (GameJam) Game.Current;

    public IAudioDevice    AudioDevice    { get; private set; } = null!;
    public IComputeDevice  ComputeDevice  { get; private set; } = null!;
    public IGraphicsDevice GraphicsDevice { get; private set; } = null!;
    public IInputManager   InputManager   { get; private set; } = null!;
    public IKeyboardDevice Keyboard       { get; private set; } = null!;
    public IMouseDevice    Mouse          { get; private set; } = null!;
    public IScreenManager  Screens        { get; private set; } = null!;
    public IGameConsole    Console        { get; private set; } = null!;
    public SpriteBatch     SpriteBatch    { get; private set; } = null!;
    public GeometryBatch   GeometryBatch  { get; private set; } = null!;

    public virtual int      SpriteCountHint  => 200;
    public virtual bool     EnableDebugTools => Debugger.IsAttached;
    public virtual LogLevel DefaultLogLevel  => LogLevel.Trace;
    public virtual Color    ClearColor       => Color.Black;

    protected override void Initialize() {
      Console = new GameConsole(new ConsoleInterpreter(RegisterConsoleBindings));

      LogFactory.Current = new CompositeLogFactory(
          new ConsoleLogFactory(DefaultLogLevel),
          new DebugLogFactory(DefaultLogLevel),
          new GameConsoleLogFactory(Console, DefaultLogLevel)
      );

      AudioDevice    = Host.Services.GetRequiredService<IAudioDevice>();
      ComputeDevice  = Host.Services.GetRequiredService<IComputeDevice>();
      GraphicsDevice = Host.Services.GetRequiredService<IGraphicsDevice>();

      Screens = new ScreenManager(this);

      Plugins.Add(Screens);

      if (EnableDebugTools) {
        Plugins.Add(new GameConsolePlugin(this));
        Plugins.Add(new ProfilerPlugin(this));
      }

      RegisterAssetLoaders(Assets);

      base.Initialize();

      OnResized(Host.Width, Host.Height); // initial resize
    }

    protected override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);

      SpriteBatch   = await SpriteBatch.CreateDefaultAsync(GraphicsDevice);
      GeometryBatch = await GeometryBatch.CreateDefaultAsync(GraphicsDevice);
    }

    protected override void RegisterServices(IServiceContainer services) {
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

    protected virtual void RegisterAssetLoaders(AssetManager assets) {
      assets.RegisterLoader(new AudioClip.Loader(AudioDevice));
      assets.RegisterLoader(new BitmapFont.Loader(CharacterSet.Standard));
      assets.RegisterLoader(new TrueTypeFont.Loader());
      assets.RegisterLoader(new ColorPalette.Loader());
      assets.RegisterLoader(new ComputeProgram.Loader(ComputeDevice));
      assets.RegisterLoader(new Image.Loader());
      assets.RegisterLoader(new ImageRegion.Loader());
      assets.RegisterLoader(new ShaderProgram.Loader(GraphicsDevice, new SpirvShaderCompiler(), hotReloading: EnableDebugTools));
      assets.RegisterLoader(new Texture.Loader(GraphicsDevice));
      assets.RegisterLoader(new TextureRegion.Loader());
      assets.RegisterLoader(new AudioBuffer.Loader());
    }

    protected virtual void RegisterConsoleBindings(IConsoleInterpreterBindings bindings) {
      bindings.Add("exit", Exit);
      bindings.Add("clear", () => Console.Clear());
    }

    protected override void OnResized(int width, int height) {
      base.OnResized(width, height);

      GraphicsDevice.Viewport = new Viewport(width, height);
    }

    protected override void Begin(GameTime time) {
      GraphicsDevice.BeginFrame();

      if (ClearColor != Color.Clear) {
        GraphicsDevice.Clear(ClearColor);
      }

      base.Begin(time);
    }

    protected override void End(GameTime time) {
      base.End(time);

      GraphicsDevice.EndFrame();
      GraphicsDevice.Present();
    }

    public override void Dispose() {
      GeometryBatch.Dispose();
      SpriteBatch.Dispose();

      base.Dispose();
    }
  }

  public abstract class GameJam<TSelf> : GameJam
      where TSelf : GameJam<TSelf> {
    public new static TSelf Current => (TSelf) GameJam.Current;
  }
}