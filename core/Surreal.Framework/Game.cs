using Surreal.Assets;
using Surreal.Audio;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Graphics;
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
/// A function that runs the game loop.
/// </summary>
public delegate void GameLoop(GameTime time);

/// <summary>
/// Entry point for the game.
/// </summary>
public class Game : IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<Game>();
  private static readonly ConcurrentQueue<Action> Callbacks = new();

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
  /// Starts the game.
  /// </summary>
  public static int Start(GameConfiguration configuration, Delegate setup)
  {
    return Run(GameContext.Current, configuration, async game =>
    {
      var result = game.Services.ExecuteDelegate(setup, game);
      if (result is Task task)
      {
        await task;
      }
    });
  }

  /// <summary>
  /// Runs this game inside a <see cref="GameContext"/>.
  /// </summary>
  [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
  private static int Run(GameContext context, GameConfiguration configuration, Func<Game, Task> gameSetup)
  {
    SynchronizationContext.SetSynchronizationContext(new GameAffineSynchronizationContext());

    try
    {
      context.OnStarted();

      var startTime = TimeStamp.Now;

      using var services = new ServiceRegistry();
      using var host = context.PlatformHost ?? configuration.Platform.BuildHost();
      using var game = new Game { Services = services, Host = host };

      context.Cancelled += () => game.Exit();
      context.HotReloaded += () => Log.Trace("The game has hot reloaded!");

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

      using var audioDevice = audioBackend.CreateDevice();
      using var graphicsDevice = graphicsBackend.CreateDevice(configuration.GraphicsMode);

      services.AddService(audioDevice);
      services.AddService(graphicsDevice);

      foreach (var device in inputBackend.Devices)
      {
        services.AddService(device.Type, device);
      }

      // resize the viewport
      host.Resized += OnHostResized;

      // register asset loaders
      foreach (var loader in services.GetServices<IAssetLoader>())
      {
        game.Assets.AddLoader(loader);
      }

      // prepare the game setup on the first frame of the loop
      Schedule(async () =>
      {
        try
        {
          await gameSetup(game);
        }
        catch (Exception exception)
        {
          Log.Error(exception, $"An unhandled top-level exception occurred");

          game.Exit();
        }
      });

      Log.Trace($"Startup took {TimeStamp.Now - startTime:g}");
      Log.Trace("Starting event loop");

      PumpEventLoop(); // start the event loop running

      Log.Trace("Exiting event loop");
      Callbacks.Clear();

      host.Resized -= OnHostResized;

      void OnHostResized(int newWidth, int newHeight)
      {
        graphicsDevice.SetViewportSize(new Viewport(0, 0, (uint)newWidth, (uint)newHeight));
      }
    }
    finally
    {
      context.OnStopped();
    }

    return 0;
  }

  /// <summary>
  /// Schedules a function to be invoked at the start of the next frame.
  /// </summary>
  public static void Schedule(Action callback)
  {
    Callbacks.Enqueue(callback);
  }

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
  /// A callback for updating the game.
  /// </summary>
  public event Action<GameTime>? Update;

  /// <summary>
  /// A callback for rendering the game.
  /// </summary>
  public event Action<GameTime>? Render;

  /// <summary>
  /// Executes the game with a variable step frequency.
  /// </summary>
  public async Task ExecuteAsync(bool runInBackground = false)
  {
    var startTime = TimeStamp.Now;

    Host.Update += OnHostUpdate;
    Host.Render += OnHostRender;

    try
    {
      await Host.RunAsync();
    }
    finally
    {
      Host.Update -= OnHostUpdate;
      Host.Render -= OnHostRender;
    }

    void OnHostUpdate(DeltaTime deltaTime)
    {
      var time = new GameTime { DeltaTime = deltaTime, TotalTime = TimeStamp.Now - startTime };

      if (Host.IsFocused || runInBackground)
      {
        Update?.Invoke(time);
      }

      PumpEventLoop();
    }

    void OnHostRender(DeltaTime deltaTime)
    {
      var time = new GameTime { DeltaTime = deltaTime, TotalTime = TimeStamp.Now - startTime };

      if (Host.IsVisible || runInBackground)
      {
        Render?.Invoke(time);
      }
    }
  }

  /// <summary>
  /// Exits the game at the end of the frame.
  /// </summary>
  public void Exit()
  {
    Host.Close();
  }

  /// <summary>
  /// Pumps the main event loop a single frame.
  /// </summary>
  protected static void PumpEventLoop()
  {
    while (Callbacks.TryDequeue(out var callback))
    {
      callback.Invoke();
    }
  }

  public void Dispose()
  {
    Assets.Dispose();
  }

  /// <summary>
  /// Synchronizes back to the main <see cref="Game"/>.
  /// </summary>
  private sealed class GameAffineSynchronizationContext : SynchronizationContext
  {
    public override void Post(SendOrPostCallback callback, object? state)
    {
      Schedule(() => callback(state));
    }

    public override void Send(SendOrPostCallback callback, object? state)
    {
      Schedule(() => callback(state));
    }
  }
}
