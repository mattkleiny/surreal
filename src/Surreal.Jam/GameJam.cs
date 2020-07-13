using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Audio.SPI;
using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Compute.SPI;
using Surreal.Diagnostics.Console;
using Surreal.Diagnostics.Console.Interpreter;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics;
using Surreal.Graphics.Experimental.Shady;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.SPI;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;

namespace Surreal {
  public abstract class GameJam : Game {
    public IAudioDevice    AudioDevice    { get; private set; } = null!;
    public IComputeDevice  ComputeDevice  { get; private set; } = null!;
    public IGraphicsDevice GraphicsDevice { get; private set; } = null!;
    public IInputManager   InputManager   { get; private set; } = null!;
    public IKeyboardDevice Keyboard       { get; private set; } = null!;
    public IMouseDevice    Mouse          { get; private set; } = null!;
    public IScreenManager  Screens        { get; private set; } = null!;
    public IGameConsole    Console        { get; private set; } = null!;
    public ModdingPlugin   Mods           { get; private set; } = null!;
    public SpriteBatch     SpriteBatch    { get; private set; } = null!;
    public GeometryBatch   GeometryBatch  { get; private set; } = null!;

    public virtual int      AudioSourceHint  => 256;
    public virtual int      SpriteCountHint  => 200;
    public virtual bool     EnableDebugTools => Debugger.IsAttached;
    public virtual LogLevel DefaultLogLevel  => LogLevel.Trace;

    protected override void Initialize() {
      Console = new GameConsole(new ConsoleInterpreter(RegisterConsoleBindings));

      LogFactory.Current = new CompositeLogFactory(
          new ConsoleLogFactory(DefaultLogLevel),
          new DebugLogFactory(DefaultLogLevel),
          new GameConsoleLogFactory(Console, DefaultLogLevel)
      );

      AudioDevice    = CreateAudioDevice(Host.Services.GetRequiredService<IAudioBackend>());
      ComputeDevice  = CreateComputeDevice(Host.Services.GetRequiredService<IComputeBackend>());
      GraphicsDevice = CreateGraphicsDevice(Host.Services.GetRequiredService<IGraphicsBackend>());

      Screens = new ScreenManager(this);
      Mods    = new ModdingPlugin(this);

      Plugins.Add(Screens);
      Plugins.Add(Mods);

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

      SpriteBatch   = await CreateSpriteBatchAsync(SpriteCountHint);
      GeometryBatch = await GeometryBatch.CreateDefaultAsync(GraphicsDevice);
    }

    protected virtual IAudioDevice CreateAudioDevice(IAudioBackend backend) {
      return new AudioDevice(backend, AudioSourceHint) {
          MasterVolume = 1f
      };
    }

    protected virtual IComputeDevice CreateComputeDevice(IComputeBackend backend) {
      return new ComputeDevice(backend);
    }

    protected virtual IGraphicsDevice CreateGraphicsDevice(IGraphicsBackend backend) {
      return new GraphicsDevice(backend, Host) {
          Pipeline = {
              Rasterizer = {
                  Viewport              = new Viewport(Host.Width, Host.Height),
                  IsBlendingEnabled     = true,
                  IsDepthTestingEnabled = false
              }
          }
      };
    }

    protected virtual async Task<SpriteBatch> CreateSpriteBatchAsync(int spriteCountHint) {
      return await SpriteBatch.CreateDefaultAsync(GraphicsDevice, spriteCountHint);
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
      assets.RegisterLoader(new BitmapFont.Loader());
      assets.RegisterLoader(new TrueTypeFont.Loader());
      assets.RegisterLoader(new ColorPalette.Loader());
      assets.RegisterLoader(new ComputeProgram.Loader(ComputeDevice));
      assets.RegisterLoader(new Image.Loader());
      assets.RegisterLoader(new ShaderProgram.Loader(GraphicsDevice, hotReloading: EnableDebugTools));
      assets.RegisterLoader(new ShadyProgram.Loader());
      assets.RegisterLoader(new Texture.Loader(GraphicsDevice));
      assets.RegisterLoader(new WaveData.Loader());
    }

    protected virtual void RegisterConsoleBindings(IConsoleInterpreterBindings bindings) {
      bindings.Add("exit", Exit);
      bindings.Add("clear", () => Console.Clear());
    }

    protected override void OnResized(int width, int height) {
      base.OnResized(width, height);

      GraphicsDevice.Viewport = new Viewport(width, height);
    }

    public override void Dispose() {
      GeometryBatch.Dispose();
      SpriteBatch.Dispose();

      base.Dispose();
    }
  }

  public abstract class GameJam<TSelf> : GameJam
      where TSelf : GameJam<TSelf> {
    public static TSelf Current { get; private set; } = null!;

    protected GameJam() {
      Current = (TSelf) this;
    }
  }
}