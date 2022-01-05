using System.ComponentModel.Design;
using System.Runtime;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;
using Surreal.Fibers;
using Surreal.IO;
using Surreal.Timing;
using Stopwatch = Surreal.Timing.Stopwatch;

namespace Surreal;

public delegate GameLoopDelegate GameSetupDelegate(IPlatformHost host);
public delegate FiberTask        GameLoopDelegate(GameContext context);

/// <summary>Context for per-frame game loop updates.</summary>
public readonly record struct GameContext(IPlatformHost Host, GameTime GameTime);

/// <summary>Base class for any game built with Surreal.</summary>
public abstract class Game : IDisposable
{
  private static readonly IProfiler               Profiler = ProfilerFactory.GetProfiler<Game>();
  private static readonly ConcurrentQueue<Action> Actions  = new();

  private readonly TimeStamp   startTime = TimeStamp.Now;
  private readonly ILoopTarget loopTarget;

  /// <summary>Bootstraps a delegate-based game with the given <see cref="platform"/>.</summary>
  public static void Start(IPlatform platform, GameSetupDelegate gameSetup)
  {
    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

    using var host = platform.BuildHost();

    var stopwatch = new Stopwatch();
    var startTime = TimeStamp.Now;
    var gameLoop  = gameSetup(host);

    while (!host.IsClosing)
    {
      var deltaTime = stopwatch.Tick();
      var totalTime = TimeStamp.Now - startTime;

      host.Tick(deltaTime);
      FiberScheduler.Tick();

      var gameTime = new GameTime(
        DeltaTime: deltaTime,
        TotalTime: totalTime,
        IsRunningSlowly: deltaTime > 32.Milliseconds()
      );

      gameLoop(new GameContext(host, gameTime));

      Thread.Yield();
    }
  }

  public static TGame Create<TGame>(Configuration configuration)
    where TGame : Game, new()
  {
    if (configuration.Platform == null)
    {
      throw new InvalidOperationException("A valid platform was expected to be set in configuration.");
    }

    return new TGame
    {
      Host = configuration.Platform.BuildHost(),
    };
  }

  public static async Task StartAsync<TGame>(Configuration configuration)
    where TGame : Game, new()
  {
    using var game = Create<TGame>(configuration);

    await game.InitializeAsync();
    await game.RunAsync();
  }

  public static void Schedule(Action callback)
  {
    Actions.Enqueue(callback);
  }

  protected Game()
  {
    loopTarget = new ProfiledLoopTarget(this);
  }

  public bool              IsRunning    { get; private set; }  = false;
  public IPlatformHost     Host         { get; private init; } = null!;
  public IAssetManager     Assets       { get; }               = new AssetManager();
  public IServiceContainer Services     { get; }               = new ServiceContainer();
  public ILoopStrategy     LoopStrategy { get; set; }          = new AveragingLoopStrategy();
  public List<IGamePlugin> Plugins      { get; }               = new();

  private async Task RunAsync()
  {
    // TODO: make this properly async

    if (IsRunning)
    {
      throw new InvalidOperationException("The engine is already running, and cannot start again!");
    }

    try
    {
      GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

      IsRunning = true;

      var stopwatch = new Stopwatch();

      while (IsRunning && !Host.IsClosing)
      {
        var deltaTime = stopwatch.Tick();
        var totalTime = TimeStamp.Now - startTime;

        Host.Tick(deltaTime);
        LoopStrategy.Tick(loopTarget, deltaTime, totalTime);

        FiberScheduler.Tick();
      }
    }
    finally
    {
      IsRunning = false;
    }
  }

  private async Task InitializeAsync()
  {
    Initialize();

    for (var i = 0; i < Plugins.Count; i++)
    {
      await Plugins[i].InitializeAsync();
    }

    await LoadContentAsync(Assets);
  }

  protected virtual void Initialize()
  {
    Host.Resized += OnResized;

    RegisterFileSystems(FileSystem.Registry);
    RegisterServices(Services);
  }

  protected virtual async Task LoadContentAsync(IAssetContext assets)
  {
    foreach (var plugin in Plugins)
    {
      await plugin.LoadContentAsync(assets);
    }
  }

  protected virtual void RegisterServices(IServiceContainer services)
  {
    services.AddService(Assets);
    services.AddService(Host);
  }

  protected virtual void RegisterFileSystems(IFileSystemRegistry registry)
  {
    if (Host.Services.TryGetService(out IFileSystem platformFileSystem))
    {
      registry.Add(platformFileSystem);
    }

    registry.Add(new ResourceFileSystem());
  }

  protected virtual void Begin(GameTime time)
  {
  }

  protected virtual void Input(GameTime time)
  {
    foreach (var plugin in Plugins)
    {
      plugin.Input(time);
    }
  }

  protected virtual void Update(GameTime time)
  {
    foreach (var plugin in Plugins)
    {
      plugin.Update(time);
    }

    while (Actions.TryDequeue(out var action))
    {
      action.Invoke();
    }
  }

  protected virtual void Draw(GameTime time)
  {
    foreach (var plugin in Plugins)
    {
      plugin.Draw(time);
    }
  }

  protected virtual void End(GameTime time)
  {
  }

  protected virtual void OnResized(int width, int height)
  {
  }

  public void Exit()
  {
    IsRunning = false;
  }

  public virtual void Dispose()
  {
    foreach (var plugin in Plugins)
    {
      plugin.Dispose();
    }

    if (Services is IDisposable services)
    {
      services.Dispose();
    }

    Host.Dispose();
  }

  public interface ILoopStrategy
  {
    GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime);
  }

  public sealed class ChillLoopStrategy : ILoopStrategy
  {
    public ChillLoopStrategy(ILoopStrategy strategy)
    {
      Strategy = strategy;
    }

    public ILoopStrategy Strategy         { get; }
    public TimeSpan      MaxSleepInterval { get; set; } = 16.Milliseconds();

    public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
    {
      var time = Strategy.Tick(target, deltaTime, totalTime);

      if (!time.IsRunningSlowly)
      {
        var sleepTime = MaxSleepInterval - time.DeltaTime;
        if (sleepTime.TotalMilliseconds > 0f)
        {
          using (Profiler.Track("Chilling"))
          {
            Thread.Sleep(sleepTime);
          }
        }
      }

      return time;
    }
  }

  public sealed class AveragingLoopStrategy : ILoopStrategy
  {
    private readonly RingBuffer<TimeSpan> samples;

    public AveragingLoopStrategy(int samples = 10)
    {
      Debug.Assert(samples > 0, "samples > 0");

      this.samples = new RingBuffer<TimeSpan>(samples);
    }

    public TimeSpan TargetDeltaTime { get; } = 16.Milliseconds();

    public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
    {
      samples.Add(deltaTime);

      var averagedDeltaTime = samples.FastAverage();

      var time = new GameTime(
        DeltaTime: averagedDeltaTime,
        TotalTime: totalTime,
        IsRunningSlowly: averagedDeltaTime > TargetDeltaTime
      );

      target.Begin(time);
      target.Input(time);
      target.Update(time);
      target.Draw(time);
      target.End(time);

      return time;
    }
  }

  public sealed class VariableStepLoopStrategy : ILoopStrategy
  {
    public TimeSpan TargetDeltaTime { get; set; } = 16.Milliseconds();

    public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
    {
      var time = new GameTime(
        DeltaTime: deltaTime,
        TotalTime: totalTime,
        IsRunningSlowly: deltaTime > TargetDeltaTime
      );

      target.Begin(time);
      target.Input(time);
      target.Update(time);
      target.Draw(time);
      target.End(time);

      return time;
    }
  }

  public sealed class FixedStepLoopStrategy : ILoopStrategy
  {
    private double accumulator;

    public TimeSpan Step            { get; set; } = 16.Milliseconds();
    public TimeSpan TargetDeltaTime { get; set; } = 16.Milliseconds();

    public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
    {
      var time = new GameTime(
        DeltaTime: deltaTime,
        TotalTime: totalTime,
        IsRunningSlowly: deltaTime > TargetDeltaTime
      );

      accumulator += deltaTime.TimeSpan.TotalSeconds;

      target.Begin(time);
      target.Input(time);

      while (accumulator >= Step.TotalSeconds)
      {
        var stepTime = new GameTime(
          DeltaTime: Step,
          TotalTime: time.TotalTime,
          IsRunningSlowly: time.IsRunningSlowly
        );

        target.Update(stepTime);

        accumulator -= Step.TotalSeconds;
      }

      target.Draw(time);
      target.End(time);

      return time;
    }
  }

  public interface ILoopTarget
  {
    void Begin(GameTime time);
    void Input(GameTime time);
    void Update(GameTime time);
    void Draw(GameTime time);
    void End(GameTime time);
  }

  private sealed class ProfiledLoopTarget : ILoopTarget
  {
    private readonly Game game;

    public ProfiledLoopTarget(Game game)
    {
      this.game = game;
    }

    public void Begin(GameTime time)
    {
      using var _ = Profiler.Track(nameof(Begin));

      game.Begin(time);
    }

    public void Input(GameTime time)
    {
      using var _ = Profiler.Track(nameof(Input));

      game.Input(time);
    }

    public void Update(GameTime time)
    {
      using var _ = Profiler.Track(nameof(Update));

      game.Update(time);
    }

    public void Draw(GameTime time)
    {
      using var _ = Profiler.Track(nameof(Draw));

      game.Draw(time);
    }

    public void End(GameTime time)
    {
      using var _ = Profiler.Track(nameof(End));

      game.End(time);
    }
  }

  public sealed class Configuration
  {
    public IPlatform? Platform { get; init; }
  }
}
