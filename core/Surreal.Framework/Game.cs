using Surreal.Assets;
using Surreal.Audio;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Entities;
using Surreal.Graphics;
using Surreal.Graphics.Rendering;
using Surreal.Input;
using Surreal.Physics;
using Surreal.Scripting;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal;

/// <summary>
/// Configuration for the game.
/// </summary>
public sealed record GameConfiguration
{
  /// <summary>
  /// The <see cref="IPlatform"/> to use for the game.
  /// </summary>
  public required IPlatform Platform { get; init; }

  /// <summary>
  /// The <see cref="IServiceModule"/>s to use for the game.
  /// </summary>
  public List<IServiceModule> Modules { get; init; } =
  [
    new AudioModule(),
    new GraphicsModule(),
    new InputModule(),
    new PhysicsModule(),
    new ScriptingModule()
  ];

  /// <summary>
  /// The graphics mode to use for the game.
  /// </summary>
  public GraphicsMode GraphicsMode { get; init; } = GraphicsMode.Universal;
}

/// <summary>
/// A timing snapshot for the main game loop.
/// </summary>
public readonly record struct GameTime
{
  /// <summary>
  /// The time since the last frame.
  /// </summary>
  public required DeltaTime DeltaTime { get; init; }

  /// <summary>
  /// The total time elapsed since the game started.
  /// </summary>
  public required TimeSpan TotalTime { get; init; }
}

/// <summary>
/// Entry point for the game.
/// </summary>
public sealed class Game : IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<Game>();

  private readonly TimeStamp _startTime = TimeStamp.Now;

  /// <summary>
  /// Sets up the logging and profiling systems.
  /// </summary>
  [ModuleInitializer]
  [SuppressMessage("Usage", "CA2255:The \'ModuleInitializer\' attribute should not be used in libraries")]
  internal static void SetupLogging()
  {
    LogFactory.Current = new TextWriterLogFactory(Console.Out, LogLevel.Trace, LogFormatters.Default());
    ProfilerFactory.Current = new SamplingProfilerFactory(new InMemoryProfilerSampler());
  }

  /// <summary>
  /// Creates a new game with the given <see cref="GameConfiguration"/>.
  /// </summary>
  public static Game Create(GameConfiguration configuration)
  {
    var context = GameContext.Current;
    var services = new ServiceRegistry();
    var host = context.PlatformHost ?? configuration.Platform.BuildHost();

    // configure the game services
    host.RegisterServices(services);

    foreach (var module in configuration.Modules)
    {
      services.AddModule(module);
    }

    // create the main devices and register them
    var audioBackend = services.GetServiceOrThrow<IAudioBackend>();
    var graphicsBackend = services.GetServiceOrThrow<IGraphicsBackend>();
    var inputBackend = services.GetServiceOrThrow<IInputBackend>();

    var game = new Game
    {
      Services = services,
      Host = host,
      Audio = audioBackend.CreateDevice(),
      Graphics = graphicsBackend.CreateDevice(configuration.GraphicsMode)
    };

    services.AddService(game.Audio);
    services.AddService(game.Graphics);

    foreach (var device in inputBackend.Devices)
    {
      services.AddService(device.Type, device);
    }

    foreach (var loader in services.GetServices<IAssetLoader>())
    {
      game.Assets.AddLoader(loader);
    }

    // resize the viewport
    host.Update += game.OnHostUpdate;
    host.Render += game.OnHostRender;
    host.Resized += game.OnHostResized;

    context.Cancelled += () => game.Exit();
    context.HotReloaded += () => Log.Trace("The game has hot reloaded!");

    return game;
  }

  /// <summary>
  /// An event that is called every frame.
  /// </summary>
  public event Action<GameTime>? VariableTick;

  /// <summary>
  /// An event that is called at a fixed rate per frame.
  /// </summary>
  public event Action<GameTime>? FixedTick;

  /// <summary>
  /// An event that is called every frame to render the game.
  /// </summary>
  public event Action<GameTime>? Render;

  /// <summary>
  /// The assets available to the game.
  /// </summary>
  public AssetManager Assets { get; } = new();

  /// <summary>
  /// The services available to the game.
  /// </summary>
  public required ServiceRegistry Services { get; init; }

  /// <summary>
  /// The main platform host for the game.
  /// </summary>
  public required IPlatformHost Host { get; init; }

  /// <summary>
  /// The audio device for the game.
  /// </summary>
  public required IAudioDevice Audio { get; init; }

  /// <summary>
  /// The graphics device for the game.
  /// </summary>
  public required IGraphicsDevice Graphics { get; init; }

  /// <summary>
  /// The fixed tick rate for the game.
  /// </summary>
  public DeltaTime FixedTickRate { get; init; } = new DeltaTime(1f / 60f);

  /// <summary>
  /// Starts the game.
  /// </summary>
  public async Task RunAsync()
  {
    Log.Trace("Starting main loop");

    await Host.RunAsync();

    Log.Trace("Exiting main loop");
  }

  /// <summary>
  /// Starts the game with the given <see cref="EntityWorld"/>.
  /// </summary>
  public async Task RunAsync(EntityWorld world)
  {
    using var contexts = new RenderContextManager();

    VariableTick += time =>
    {
      world.Execute(new VariableTick(time.DeltaTime));
    };

    FixedTick += time =>
    {
      world.Execute(new FixedTick(time.DeltaTime));
    };

    Render += time =>
    {
      world.Execute(new RenderFrame
      {
        DeltaTime = time.DeltaTime,
        Device = Graphics,
        Contexts = contexts,
        Viewport = Graphics.GetViewportSize()
      });
    };

    Log.Trace("Starting main loop with world");

    await Host.RunAsync();

    Log.Trace("Exiting main loop with world");
  }

  /// <summary>
  /// Exits the game at the end of the frame.
  /// </summary>
  public void Exit()
  {
    Host.Close();
  }

  private void OnHostUpdate(DeltaTime deltaTime)
  {
    var time = new GameTime
    {
      DeltaTime = deltaTime,
      TotalTime = TimeStamp.Now - _startTime
    };

    if (Host.IsFocused)
    {
      VariableTick?.Invoke(time);

      if (FixedTick != null)
      {
        var remainingTicks = time.DeltaTime;

        while (remainingTicks > FixedTickRate)
        {
          FixedTick.Invoke(new GameTime
          {
            DeltaTime = FixedTickRate,
            TotalTime = time.TotalTime
          });

          remainingTicks -= FixedTickRate;
        }
      }
    }
  }

  private void OnHostRender(DeltaTime deltaTime)
  {
    var time = new GameTime
    {
      DeltaTime = deltaTime,
      TotalTime = TimeStamp.Now - _startTime
    };

    if (Host.IsVisible)
    {
      Render?.Invoke(time);
    }
  }

  private void OnHostResized(int newWidth, int newHeight)
  {
    if (Services.TryGetService(out IGraphicsDevice graphicsDevice))
    {
      graphicsDevice.SetViewportSize(new Viewport(0, 0, (uint)newWidth, (uint)newHeight));
    }
  }

  public void Dispose()
  {
    Host.Update -= OnHostUpdate;
    Host.Render -= OnHostRender;
    Host.Resized -= OnHostResized;

    Graphics.Dispose();
    Audio.Dispose();
    Assets.Dispose();
    Host.Dispose();
    Services.Dispose();
  }
}
