using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using Surreal.Assets;
using Surreal.Diagnostics.Logging;
using Surreal.IO;
using Surreal.Timing;

namespace Surreal;

/// <summary>Encapsulates the frame-by-frame timing information for a game.</summary>
[DebuggerDisplay("{DeltaTime} since last frame")]
public readonly record struct GameTime(DeltaTime DeltaTime, TimeSpan TotalTime, bool IsRunningSlowly);

/// <summary>Entry point for configuring a game.</summary>
public delegate ValueTask GameSetup(Game context);

/// <summary>Invoked to execute a single frame of a game's main loop.</summary>
public delegate void GameLoop(GameTime time);

/// <summary>Entry point for the game.</summary>
public sealed class Game : IDisposable
{
  private readonly ConcurrentQueue<Action> callbacks = new();

  private Game(IServiceRegistry services, IPlatformHost host)
  {
    Services = services;
    Host     = host;
  }

  [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
  public static void Start(IPlatform platform, GameSetup gameSetup, CancellationToken cancellationToken = default)
  {
    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

    LogFactory.Current = new CompositeLogFactory(
      new TextWriterLogFactory(Console.Out, LogLevel.Trace),
      new DebugLogFactory(LogLevel.Trace)
    );

    using var host = platform.BuildHost();
    using var services = new ServiceRegistry();
    using var game = new Game(services, host);

    SynchronizationContext.SetSynchronizationContext(new GameSynchronizationContext(game));

    cancellationToken.Register(() => game.IsClosing = true);

    host.RegisterServices(services);
    host.RegisterAssetLoaders(game.Assets);
    host.RegisterFileSystems(FileSystem.Registry);

    game.Schedule(() => gameSetup(game));

    game.Run();
  }

  public IServiceRegistry Services { get; init; }
  public IPlatformHost    Host     { get; init; }
  public IAssetManager    Assets   { get; } = new AssetManager();

  /// <summary>True if the current <see cref="ExecuteVariableStep"/> block is closing.</summary>
  public bool IsClosing { get; private set; } = false;

  /// <summary>Schedules an action to be invoked at the start of the next frame.</summary>
  public void Schedule(Action callback)
  {
    callbacks.Enqueue(callback);
  }

  /// <summary>Runs the main event loop for the game.</summary>
  private void Run()
  {
    while (!Host.IsClosing && !IsClosing)
    {
      while (callbacks.TryDequeue(out var callback))
      {
        callback.Invoke();
      }
    }
  }

  /// <summary>Executes the given <see cref="gameLoop"/> with a variable step frequency.</summary>
  public void ExecuteVariableStep(GameLoop gameLoop)
  {
    IsClosing = false;

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
      gameLoop(gameTime);
      Host.EndFrame(gameTime.DeltaTime);
    }
  }

  /// <summary>Executes the given delegates with a fixed stepping interval on the <see cref="physics"/>.</summary>
  public void ExecuteFixedStep(GameLoop physics, GameLoop render)
  {
    IsClosing = false;

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

      accumulator += gameTime.DeltaTime;

      Host.BeginFrame(gameTime.DeltaTime);

      // run the physics steps
      while (accumulator > 0f)
      {
        physics(gameTime with
        {
          TotalTime = TimeStamp.Now - startTime,
        });

        accumulator -= gameTime.DeltaTime;
      }

      // run the render steps
      render(gameTime);

      Host.EndFrame(gameTime.DeltaTime);
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
