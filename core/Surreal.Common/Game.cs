using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using Surreal.Assets;
using Surreal.Diagnostics.Logging;
using Surreal.IO;
using Surreal.Timing;

namespace Surreal;

/// <summary>Encapsulates the frame-by-frame timing information for a game.</summary>
[DebuggerDisplay("{DeltaTime} since last frame")]
public readonly record struct GameTime(
  TimeDelta DeltaTime,
  TimeDelta TotalTime,
  bool IsRunningSlowly
);

/// <summary>Entry point for the game.</summary>
public sealed class Game : IDisposable
{
  public delegate Task GameSetup(Game context);
  public delegate void GameLoop(GameTime time);

  private static readonly ILog Log = LogFactory.GetLog<Game>();

  private readonly ConcurrentQueue<Action> callbacks = new();

  private Game(IServiceRegistry services, IPlatformHost host)
  {
    Services = services;
    Host     = host;
  }

  [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
  [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
  public static void Start(IPlatform platform, GameSetup gameSetup, CancellationToken cancellationToken = default)
  {
    // configure gc and logging
    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

    LogFactory.Current = new CompositeLogFactory(
      new TextWriterLogFactory(Console.Out, LogLevel.Trace),
      new TraceLogFactory(LogLevel.Trace)
    );

    // prepare core services
    using var host = platform.BuildHost();
    using var services = new ServiceRegistry();
    using var game = new Game(services, host);

    // marshal all async work back to the main thread
    SynchronizationContext.SetSynchronizationContext(new GameSynchronizationContext(game));

    // allow early termination of the core event loop
    cancellationToken.Register(() => game.IsClosing = true);

    // prepare the game host
    host.RegisterServices(services);
    host.RegisterAssetLoaders(game.Assets);
    host.RegisterFileSystems(FileSystem.Registry);

    // prepare the game and loop
    game.Schedule(() => gameSetup(game).ContinueWith(task =>
    {
      if (task.IsFaulted)
      {
        Log.Error((string) $"An unhandled top-level exception occurred: {task.Exception}");

        game.Exit();
      }
    }));

    // run the game
    game.RunEventLoop();
  }

  public IServiceRegistry Services { get; init; }
  public IPlatformHost    Host     { get; init; }
  public IAssetManager    Assets   { get; } = new AssetManager();

  /// <summary>True if the game is getting ready to close.</summary>
  public bool IsClosing { get; private set; } = false;

  /// <summary>Schedules an action to be invoked on the event loop.</summary>
  public void Schedule(Action callback)
  {
    callbacks.Enqueue(callback);
  }

  /// <summary>Runs the main event loop for the game.</summary>
  private void RunEventLoop()
  {
    while (!Host.IsClosing && !IsClosing)
    {
      // eventually this will end up blocking when a main loop takes over
      PumpEventLoop();
    }
  }

  /// <summary>Pumps the main event loop a single frame.</summary>
  private void PumpEventLoop()
  {
    while (callbacks.TryDequeue(out var callback))
    {
      callback.Invoke();
    }
  }

  /// <summary>Executes the given <see cref="gameLoop"/> with a variable step frequency.</summary>
  public void ExecuteVariableStep(GameLoop gameLoop, bool runInBackground = false)
  {
    var stopwatch = new Chronometer();
    var startTime = TimeStamp.Now;

    while (!Host.IsClosing && !IsClosing)
    {
      // calculate frame times
      var gameTime = new GameTime(
        DeltaTime: stopwatch.Tick(),
        TotalTime: TimeStamp.Now - startTime,
        IsRunningSlowly: stopwatch.Tick() > 32.Milliseconds()
      );

      // run the frame logic
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

  /// <summary>Executes the given delegates with a fixed stepping interval on the <see cref="physics"/>.</summary>
  public void ExecuteFixedStep(GameLoop physics, GameLoop render, bool runInBackground = false)
  {
    var stopwatch = new Chronometer();
    var startTime = TimeStamp.Now;
    var accumulator = 0.0f;

    while (!Host.IsClosing && !IsClosing)
    {
      // calculate frame times
      var gameTime = new GameTime(
        DeltaTime: stopwatch.Tick(),
        TotalTime: TimeStamp.Now - startTime,
        IsRunningSlowly: stopwatch.Tick() > 32.Milliseconds()
      );

      Host.BeginFrame(gameTime.DeltaTime);

      if (Host.IsFocused || runInBackground)
      {
        accumulator += gameTime.DeltaTime;

        // run the physics steps
        while (accumulator > 0f)
        {
          physics(gameTime with
          {
            TotalTime = TimeStamp.Now - startTime
          });

          accumulator -= gameTime.DeltaTime;
        }

        // run the render steps
        render(gameTime);
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
  private sealed class GameSynchronizationContext : SynchronizationContext
  {
    private readonly Game game;

    public GameSynchronizationContext(Game game)
    {
      this.game = game;
    }

    public override void Post(SendOrPostCallback callback, object? state)
    {
      game.Schedule(() => callback(state));
    }

    public override void Send(SendOrPostCallback callback, object? state)
    {
      game.Schedule(() => callback(state));
    }
  }
}
