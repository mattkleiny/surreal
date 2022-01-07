using System.Runtime;
using Surreal.Assets;
using Surreal.Diagnostics.Profiling;
using Surreal.Fibers;
using Surreal.IO;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal;

/// <summary>Base class for any game built with Surreal.</summary>
public abstract partial class Game : IDisposable, ITestableGame
{
  private static readonly IProfiler               Profiler = ProfilerFactory.GetProfiler<Game>();
  private static readonly ConcurrentQueue<Action> Actions  = new();

  private readonly TimeStamp   startTime = TimeStamp.Now;
  private readonly ILoopTarget loopTarget;

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

  public static Task StartAsync<TGame>(Configuration configuration, CancellationToken cancellationToken = default)
    where TGame : Game, new()
  {
    using var game = Create<TGame>(configuration);

    return game.StartAsync(cancellationToken);
  }

  public static void Schedule(Action callback)
  {
    Actions.Enqueue(callback);
  }

  protected Game() => loopTarget = new ProfiledLoopTarget(this);

  public bool              IsRunning    { get; private set; }  = false;
  public IPlatformHost     Host         { get; private init; } = null!;
  public IServiceProvider  Services     { get; private set; }  = null!;
  public IAssetManager     Assets       { get; }               = new AssetManager();
  public ILoopStrategy     LoopStrategy { get; set; }          = new AveragingLoopStrategy();
  public List<IGamePlugin> Plugins      { get; }               = new();

  private async Task StartAsync(CancellationToken cancellationToken = default)
  {
    await InitializeAsync(cancellationToken);
    await RunAsync(cancellationToken);
  }

  private async Task InitializeAsync(CancellationToken cancellationToken = default)
  {
    Initialize();

    foreach (var plugin in Plugins)
    {
      await plugin.InitializeAsync(cancellationToken);
    }

    await LoadContentAsync(Assets, cancellationToken);
  }

  private async Task RunAsync(CancellationToken cancellationToken = default)
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

      var stopwatch = new Chronometer();

      while (IsRunning && !Host.IsClosing && !cancellationToken.IsCancellationRequested)
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

  protected virtual void Initialize()
  {
    Host.Resized += OnResized;

    var registry = new ServiceCollectionRegistry();

    RegisterFileSystems(FileSystem.Registry);
    RegisterServices(registry);

    Services = registry.BuildServiceProvider();
  }

  protected virtual async Task LoadContentAsync(IAssetManager assets, CancellationToken cancellationToken = default)
  {
    foreach (var plugin in Plugins)
    {
      await plugin.LoadContentAsync(assets, cancellationToken);
    }
  }

  protected virtual void RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton(Assets);
    services.AddSingleton(Host);
  }

  protected virtual void RegisterFileSystems(IFileSystemRegistry registry)
  {
    if (Host.Services.TryGetService(out IFileSystem platformFileSystem))
    {
      registry.Add(platformFileSystem);
    }
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

  ILoopTarget ITestableGame.LoopTarget => loopTarget;

  Task ITestableGame.InitializeAsync(CancellationToken cancellationToken) => InitializeAsync(cancellationToken);
  Task ITestableGame.RunAsync(CancellationToken cancellationToken)        => RunAsync(cancellationToken);

  /// <summary>Configuration for the <see cref="Game"/>.</summary>
  public sealed class Configuration
  {
    public IPlatform? Platform { get; init; }
  }

  /// <summary>A <see cref="ILoopTarget"/> that profiles it's target operations.</summary>
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
}
