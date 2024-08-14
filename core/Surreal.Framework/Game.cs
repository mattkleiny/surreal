using System.Runtime;
using Surreal.Assets;
using Surreal.Audio;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Graphics;
using Surreal.Hosting;
using Surreal.Input;
using Surreal.Networking;
using Surreal.Physics;
using Surreal.Scripting;
using Surreal.Services;
using Surreal.Timing;

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
  public required TimeSpan TotalTime { get; init; }
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
  /// The <see cref="IServiceModule"/>s to use for the game.
  /// </summary>
  public List<IServiceModule> Modules { get; init; } =
  [
    new AudioModule(),
    new GraphicsModule(),
    new InputModule(),
    new NetworkingModule(),
    new PhysicsModule(),
    new ScriptingModule()
  ];
}

/// <summary>
/// Entry point for the game.
/// </summary>
[ExcludeFromCodeCoverage]
public class Game : IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<Game>();
  private static readonly ConcurrentQueue<Action> Callbacks = new();

  /// <summary>
  /// A function that runs the game loop.
  /// </summary>
  public delegate void GameLoop(GameTime time);

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
    return Run(configuration, async game =>
    {
      var result = game.Services.ExecuteDelegate(setup, game);
      if (result is Task task)
      {
        await task;
      }
    });
  }

  /// <summary>
  /// Runs this game inside a <see cref="HostingContext"/>.
  /// </summary>
  [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
  private static int Run(GameConfiguration configuration, Func<Game, Task> gameSetup)
  {
    return Run(configuration, HostingContext.Current, gameSetup);
  }

  /// <summary>
  /// Runs this game inside a <see cref="HostingContext"/>.
  /// </summary>
  [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
  private static int Run(GameConfiguration configuration, HostingContext context, Func<Game, Task> gameSetup)
  {
    try
    {
      context.OnStarted();

      var startTime = TimeStamp.Now;

      // build the game using the hosting context
      using var services = new ServiceRegistry();
      using var host = context.PlatformHost ?? configuration.Platform.BuildHost();

      using var game = new Game
      {
        Services = services,
        Host = host
      };

      context.Cancelled += () => game.Exit();
      context.HotReloaded += () => Log.Trace("The game has hot reloaded!");

      // configure the game services
      host.RegisterServices(services);

      foreach (var module in configuration.Modules)
      {
        services.AddModule(module);
      }

      foreach (var loader in services.GetServices<IAssetLoader>())
      {
        game.Assets.AddLoader(loader);
      }

      // marshal all async work back to the main thread
      SynchronizationContext.SetSynchronizationContext(new GameAffineSynchronizationContext());

      // prepare the game setup on the first frame of the loop
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

      Log.Trace($"Startup took {TimeStamp.Now - startTime:g}");
      Log.Trace("Running main event loop");

      while (!game.Host.IsClosing && !game.IsClosing)
      {
        // eventually this will end up blocking when a main loop takes over
        PumpEventLoop();
      }

      Log.Trace("Exiting main event loop");

      Callbacks.Clear();
    }
    finally
    {
      context.OnStopped();
    }

    Log.Trace("Completed successfully");

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
  /// True if the game is getting ready to close.
  /// </summary>
  public bool IsClosing { get; private set; }

  /// <summary>
  /// Executes the given <see cref="gameLoop"/> with a variable step frequency.
  /// </summary>
  public void ExecuteVariableStep(GameLoop gameLoop, bool runInBackground = false)
  {
    var deltaTimeClock = new DeltaTimeClock();
    var startTime = TimeStamp.Now;

    while (!Host.IsClosing && !IsClosing)
    {
      var time = new GameTime
      {
        DeltaTime = deltaTimeClock.Tick(),
        TotalTime = TimeStamp.Now - startTime
      };

      Host.BeginFrame(time.DeltaTime);

      if (Host.IsFocused || runInBackground)
      {
        gameLoop(time);
      }

      Host.EndFrame(time.DeltaTime);

      PumpEventLoop();
    }
  }

  /// <summary>
  /// Executes the given <see cref="updateLoop"/>/<see cref="drawLoop"/> combo with a fixed step frequency.
  /// </summary>
  public void ExecuteFixedStep(GameLoop updateLoop, GameLoop drawLoop, bool runInBackground = false)
  {
    var startTime = TimeStamp.Now;
    var deltaTimeClock = new DeltaTimeClock();
    var accumulator = 0f;

    while (!Host.IsClosing && !IsClosing)
    {
      var deltaTime = deltaTimeClock.Tick();

      accumulator += deltaTime;

      Host.BeginFrame(deltaTime);

      if (Host.IsFocused || runInBackground)
      {
        while (accumulator >= deltaTime)
        {
          updateLoop(new GameTime
          {
            DeltaTime = deltaTime,
            TotalTime = TimeStamp.Now - startTime
          });

          accumulator -= deltaTime;
        }

        drawLoop(new GameTime
        {
          DeltaTime = deltaTime,
          TotalTime = TimeStamp.Now - startTime
        });
      }

      Host.EndFrame(deltaTime);

      PumpEventLoop();
    }
  }

  /// <summary>
  /// Exits the game at the end of the frame.
  /// </summary>
  public void Exit()
  {
    IsClosing = true;
  }

  public void Dispose()
  {
    Assets.Dispose();
  }

  /// <summary>Pumps the main event loop a single frame.</summary>
  protected static void PumpEventLoop()
  {
    while (Callbacks.TryDequeue(out var callback))
    {
      callback.Invoke();
    }
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
