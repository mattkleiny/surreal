using System.Runtime;
using Surreal.Assets;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Internal;
using Surreal.IO;
using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

/// <summary>Base class for any game built with Surreal.</summary>
public abstract partial class Game : IDisposable, ITestableGame
{
  private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<Game>();

  private readonly TimeStamp startTime = TimeStamp.Now;
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
      ServiceOverrides = configuration.ServiceOverrides,
    };
  }

  public static async Task StartAsync<TGame>(Configuration configuration, CancellationToken cancellationToken = default)
    where TGame : Game, new()
  {
    using var game = Create<TGame>(configuration);

    await game.InitializeAsync(cancellationToken);
    await game.RunAsync(cancellationToken);
  }

  protected Game()
  {
    loopTarget = new ProfiledLoopTarget(this);
  }

  public bool          IsRunning    { get; private set; }  = false;
  public IPlatformHost Host         { get; private init; } = null!;
  public ILoopStrategy LoopStrategy { get; set; }          = new AveragingLoopStrategy();

  public IServiceRegistry    Services { get; } = new ServiceRegistry();
  public IAssetManager       Assets   { get; } = new AssetManager();
  public IGamePluginRegistry Plugins  { get; } = new GamePluginRegistry();

  [VisibleForTesting]
  private Action<IServiceRegistry>? ServiceOverrides { get; set; }

  /// <summary>Initializes the game and all of it's systems.</summary>
  public async ValueTask InitializeAsync(CancellationToken cancellationToken = default)
  {
    Initialize();

    await LoadContentAsync(Assets, cancellationToken);
  }

  /// <summary>Runs the main game loop.</summary>
  public ValueTask RunAsync(CancellationToken cancellationToken = default)
  {
    return RunAsync(Host.Dispatcher, cancellationToken);
  }

  /// <summary>Runs the main game loop.</summary>
  public async ValueTask RunAsync(IDispatcher dispatcher, CancellationToken cancellationToken = default)
  {
    if (IsRunning)
    {
      throw new InvalidOperationException("The engine is already running, and cannot start again!");
    }

    try
    {
      GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

      IsRunning = true;

      var chronometer = new Chronometer();

      while (IsRunning && !Host.IsClosing && !cancellationToken.IsCancellationRequested)
      {
        var deltaTime = chronometer.Tick();
        var totalTime = TimeStamp.Now - startTime;

        Host.Tick(deltaTime);
        LoopStrategy.Tick(loopTarget, deltaTime, totalTime);

        await dispatcher.Yield();
      }
    }
    finally
    {
      IsRunning = false;
    }
  }

  protected virtual void Initialize()
  {
    LogFactory.Current = new CompositeLogFactory(
      new TextWriterLogFactory(Console.Out, LogLevel.Trace),
      new DebugLogFactory(LogLevel.Trace)
    );

    Host.Resized += OnResized;

    RegisterServices(Services);
    RegisterAssetLoaders(Assets);
    RegisterFileSystems(FileSystem.Registry);
    RegisterPlugins(Plugins);

    Services.SealRegistry();

    foreach (var plugin in Plugins.ActivePlugins)
    {
      plugin.Initialize();
    }
  }

  protected virtual async Task LoadContentAsync(IAssetManager assets, CancellationToken cancellationToken = default)
  {
    foreach (var plugin in Plugins.ActivePlugins)
    {
      await plugin.LoadContentAsync(assets, cancellationToken);
    }
  }

  protected virtual void RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton(this);
    services.AddSingleton(Assets);
    services.AddSingleton(Host);
    services.AddSingleton(Host.Dispatcher);
    services.AddModule(Host.Services);

    ServiceOverrides?.Invoke(services);
  }

  protected virtual void RegisterAssetLoaders(IAssetManager manager)
  {
  }

  protected virtual void RegisterFileSystems(IFileSystemRegistry registry)
  {
  }

  protected virtual void RegisterPlugins(IGamePluginRegistry plugins)
  {
  }

  protected virtual void BeginFrame(GameTime time)
  {
  }

  protected virtual void Input(GameTime time)
  {
    foreach (var plugin in Plugins.ActivePlugins)
    {
      plugin.Input(time);
    }
  }

  protected virtual void Update(GameTime time)
  {
    foreach (var plugin in Plugins.ActivePlugins)
    {
      plugin.Update(time);
    }
  }

  protected virtual void Draw(GameTime time)
  {
    foreach (var plugin in Plugins.ActivePlugins)
    {
      plugin.Draw(time);
    }
  }

  protected virtual void EndFrame(GameTime time)
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
    foreach (var plugin in Plugins.ActivePlugins)
    {
      plugin.Dispose();
    }

    Assets.Dispose();
    Services.Dispose();
    Host.Dispose();
  }

  ILoopTarget ITestableGame.LoopTarget => loopTarget;

  ValueTask ITestableGame.InitializeAsync(CancellationToken cancellationToken) => InitializeAsync(cancellationToken);
  ValueTask ITestableGame.RunAsync(CancellationToken cancellationToken) => RunAsync(cancellationToken);

  /// <summary>Configuration for the <see cref="Game"/>.</summary>
  public sealed class Configuration
  {
    public IPlatform? Platform { get; init; }

    /// <summary>Custom overrides for the <see cref="IServiceRegistry"/>, to allow testing/etc.</summary>
    internal Action<IServiceRegistry>? ServiceOverrides { get; set; }
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

      game.BeginFrame(time);
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

    public void EndFrame(GameTime time)
    {
      using var _ = Profiler.Track(nameof(EndFrame));

      game.EndFrame(time);
    }
  }
}
