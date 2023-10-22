using System.Runtime;
using Surreal.Assets;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A timing snapshot for the main game loop.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct GameTime
{
  /// <summary>
  /// The time since the last frame.
  /// </summary>
  public required DeltaTime DeltaTime { get; init; }

  /// <summary>
  /// The total time elapsed since the game started.
  /// </summary>
  public required DeltaTime TotalTime { get; init; }
}

/// <summary>
/// Configuration for the game.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record GameConfiguration
{
  /// <summary>
  /// The <see cref="IPlatform"/> to use for the game.
  /// </summary>
  public required IPlatform Platform { get; init; }

  /// <summary>
  /// The root <see cref="IServiceModule"/> to use for the game.
  /// </summary>
  public IServiceModule Module { get; init; } = new FrameworkModule();
}

/// <summary>
/// Entry point for the game.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class Game : IDisposable
{
  /// <summary>
  /// A function that sets up the game.
  /// </summary>
  public delegate void GameSetup(Game game);

  /// <summary>
  /// A function that sets up the game.
  /// </summary>
  public delegate Task GameSetupAsync(Game game);

  /// <summary>
  /// A function that runs the game loop.
  /// </summary>
  public delegate void GameLoop(GameTime time);

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
  /// Schedules a function to be invoked at the start of the next frame.
  /// </summary>
  public static void Schedule(Action callback)
  {
    Callbacks.Enqueue(callback);
  }

  private Game(ServiceRegistry services, IPlatformHost host)
  {
    Services = services;
    Host = host;
  }

  /// <summary>
  /// Starts the game.
  /// </summary>
  public static void Start(GameConfiguration configuration, GameSetup gameSetup)
  {
    Start(configuration, game =>
    {
      gameSetup(game);

      return Task.CompletedTask;
    });
  }

  /// <summary>
  /// Starts the game.
  /// </summary>
  [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
  [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
  public static void Start(GameConfiguration configuration, GameSetupAsync gameSetup, CancellationToken cancellationToken = default)
  {
    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

    // prepare core services
    using var services = new ServiceRegistry();
    using var host = configuration.Platform.BuildHost(services);
    using var game = new Game(services, host);

    services.AddModule(configuration.Module);

    // prepare asset manager
    foreach (var loader in services.GetServices<IAssetLoader>())
    {
      game.Assets.AddLoader(loader);
    }

    // marshal all async work back to the main thread
    SynchronizationContext.SetSynchronizationContext(new GameAffineSynchronizationContext());

    // allow early termination of the core event loop
    cancellationToken.Register(() => game.IsClosing = true);

    // prepare the game and loop
    Schedule(() => gameSetup(game).ContinueWith(task =>
    {
      if (task.IsFaulted)
      {
        Log.Error(task.Exception is { InnerExceptions.Count: 1 }
          ? $"An unhandled top-level exception occurred: {task.Exception.InnerExceptions.Single()}"
          : $"An unhandled top-level exception occurred: {task.Exception}");

        game.Exit();
      }
    }));

    while (!game.Host.IsClosing && !game.IsClosing)
    {
      // eventually this will end up blocking when a main loop takes over
      PumpEventLoop();
    }
  }

  /// <summary>
  /// The services available to the game.
  /// </summary>
  public ServiceRegistry Services { get; init; }

  /// <summary>
  /// The assets available to the game.
  /// </summary>
  public AssetManager Assets { get; } = new();

  /// <summary>
  /// The main platform host for the game.
  /// </summary>
  public IPlatformHost Host { get; init; }

  /// <summary>
  /// True if the game is getting ready to close.
  /// </summary>
  public bool IsClosing { get; private set; }

  /// <summary>Pumps the main event loop a single frame.</summary>
  private static void PumpEventLoop()
  {
    while (Callbacks.TryDequeue(out var callback))
    {
      callback.Invoke();
    }
  }

  /// <summary>
  /// Executes the given <see cref="gameLoop"/> with a variable step frequency.
  /// </summary>
  public void ExecuteVariableStep(GameLoop gameLoop, bool runInBackground = false)
  {
    var deltaTimeClock = new DeltaTimeClock();
    var startTime = TimeStamp.Now;

    while (!Host.IsClosing && !IsClosing)
    {
      var gameTime = new GameTime
      {
        DeltaTime = deltaTimeClock.Tick(),
        TotalTime = TimeStamp.Now - startTime
      };

      Host.BeginFrame(gameTime.DeltaTime);

      if (Host.IsFocused || runInBackground)
      {
        gameLoop(gameTime);
      }

      Host.EndFrame(gameTime.DeltaTime);

      // we need to take over the event loop from here
      PumpEventLoop();
    }
  }

  /// <summary>Exits the game at the end of the frame.</summary>
  public void Exit()
  {
    IsClosing = true;
  }

  public void Dispose()
  {
    Assets.Dispose();
  }

  /// <summary>Synchronizes back to the main <see cref="Game"/>.</summary>
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
