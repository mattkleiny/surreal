using System.Runtime;
using Surreal.Assets;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Internal;
using Surreal.IO;
using Surreal.Timing;

namespace Surreal;

/// <summary>Base class for any game built with Surreal.</summary>
public abstract partial class Game : IDisposable, ITestableGame
{
  private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<Game>();

  private readonly TimeStamp startTime = TimeStamp.Now;
  private readonly Chronometer chronometer = new();
  private readonly ConcurrentQueue<Action> callbacks = new();
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
      Host             = configuration.Platform.BuildHost(),
      ServiceOverrides = configuration.ServiceOverrides,
    };
  }

  public static void Start<TGame>(Configuration configuration, CancellationToken cancellationToken = default)
    where TGame : Game, new()
  {
    using var game = Create<TGame>(configuration);

    game.Initialize(cancellationToken);
    game.Run(cancellationToken);
  }

  protected Game()
  {
    loopTarget = new ProfiledLoopTarget(this);
  }

  public bool          IsRunning    { get; private set; }  = false;
  public IPlatformHost Host         { get; private init; } = null!;
  public ILoopStrategy LoopStrategy { get; set; }          = new AveragingLoopStrategy();

  public IServiceRegistry Services { get; } = new ServiceRegistry();
  public IAssetManager    Assets   { get; } = new AssetManager();

  [VisibleForTesting]
  private Action<IServiceRegistry>? ServiceOverrides { get; set; }

  /// <summary>Initializes the game and all of it's systems.</summary>
  public void Initialize(CancellationToken cancellationToken = default)
  {
    SynchronizationContext.SetSynchronizationContext(new GameSynchronizationContext(this));

    LogFactory.Current = new CompositeLogFactory(
      new TextWriterLogFactory(Console.Out, LogLevel.Trace),
      new DebugLogFactory(LogLevel.Trace)
    );

    Host.Resized += OnResized;

    RegisterServices(Services);
    RegisterAssetLoaders(Assets);
    RegisterFileSystems(FileSystem.Registry);

    Services.SealRegistry();

    OnInitialize();

    LoadContentAsync(Assets, cancellationToken);
  }

  /// <summary>Runs the main game loop.</summary>
  public void Run(CancellationToken cancellationToken = default)
  {
    if (IsRunning)
    {
      throw new InvalidOperationException("The engine is already running, and cannot start again!");
    }

    try
    {
      GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

      IsRunning = true;

      while (IsRunning && !Host.IsClosing && !cancellationToken.IsCancellationRequested)
      {
        var deltaTime = chronometer.Tick();
        var totalTime = TimeStamp.Now - startTime;

        Host.Tick(deltaTime);
        Tick(deltaTime, totalTime);

        while (callbacks.TryDequeue(out var callback))
        {
          callback.Invoke();
        }
      }
    }
    finally
    {
      IsRunning = false;
    }
  }

  /// <summary>Ticks the game a single frame.</summary>
  public void Tick()
  {
    var deltaTime = chronometer.Tick();
    var totalTime = TimeStamp.Now - startTime;

    Tick(deltaTime, totalTime);
  }

  private void Tick(DeltaTime deltaTime, TimeSpan totalTime)
  {
    LoopStrategy.Tick(loopTarget, deltaTime, totalTime);
  }

  protected virtual void OnInitialize()
  {
  }

  protected virtual Task LoadContentAsync(IAssetManager assets, CancellationToken cancellationToken = default)
  {
    return Task.CompletedTask;
  }

  protected virtual void RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton(this);
    services.AddSingleton(Assets);
    services.AddSingleton(Host);
    services.AddModule(Host.Services);

    ServiceOverrides?.Invoke(services);
  }

  protected virtual void RegisterAssetLoaders(IAssetManager manager)
  {
  }

  protected virtual void RegisterFileSystems(IFileSystemRegistry registry)
  {
  }

  protected virtual void OnBeginFrame(GameTime time)
  {
  }

  protected virtual void OnInput(GameTime time)
  {
  }

  protected virtual void OnUpdate(GameTime time)
  {
  }

  protected virtual void OnDraw(GameTime time)
  {
  }

  protected virtual void OnEndFrame(GameTime time)
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
    Assets.Dispose();
    Services.Dispose();
    Host.Dispose();
  }

  ILoopTarget ITestableGame.LoopTarget => loopTarget;

  /// <summary>Configuration for the <see cref="Game"/>.</summary>
  public sealed class Configuration
  {
    public IPlatform? Platform { get; init; }

    /// <summary>Custom overrides for the <see cref="IServiceRegistry"/>, to allow testing/etc.</summary>
    internal Action<IServiceRegistry>? ServiceOverrides { get; set; }
  }

  /// <summary>A <see cref="SynchronizationContext"/> for the <see cref="Game"/>.</summary>
  private sealed class GameSynchronizationContext : SynchronizationContext
  {
    private readonly Game game;

    public GameSynchronizationContext(Game game)
    {
      this.game = game;
    }

    public override void Post(SendOrPostCallback callback, object? state)
    {
      game.callbacks.Enqueue(() => callback(state));
    }

    public override void Send(SendOrPostCallback callback, object? state)
    {
      game.callbacks.Enqueue(() => callback(state));
    }
  }

  /// <summary>A <see cref="ILoopTarget"/> that profiles it's target operations.</summary>
  private sealed class ProfiledLoopTarget : ILoopTarget
  {
    private readonly Game game;

    public ProfiledLoopTarget(Game game)
    {
      this.game = game;
    }

    public void BeginFrame(GameTime time)
    {
      using var _ = Profiler.Track(nameof(BeginFrame));

      game.OnBeginFrame(time);
    }

    public void Input(GameTime time)
    {
      using var _ = Profiler.Track(nameof(Input));

      game.OnInput(time);
    }

    public void Update(GameTime time)
    {
      using var _ = Profiler.Track(nameof(Update));

      game.OnUpdate(time);
    }

    public void Draw(GameTime time)
    {
      using var _ = Profiler.Track(nameof(Draw));

      game.OnDraw(time);
    }

    public void EndFrame(GameTime time)
    {
      using var _ = Profiler.Track(nameof(EndFrame));

      game.OnEndFrame(time);
    }
  }
}
