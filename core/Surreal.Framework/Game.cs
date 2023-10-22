using Surreal.Assets;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Timing;

namespace Surreal;

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
  public required DeltaTime TotalTime { get; init; }
}

/// <summary>
/// Configuration for the game.
/// </summary>
public sealed record GameConfiguration
{
  /// <summary>
  /// The <see cref="IPlatformHostFactory"/> to use for the game.
  /// </summary>
  public required IPlatformHostFactory Platform { get; init; }

  /// <summary>
  /// The <see cref="IGameHost"/> for the game.
  /// </summary>
  public required IGameHost Host { get; init; }
}

/// <summary>
/// Static facade for the game integration.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Game
{
  private static readonly ILog Log = LogFactory.GetLog<IGameHost>();

  /// <summary>
  /// Initializes the game's static state.
  /// </summary>
  [ModuleInitializer]
  [SuppressMessage("Usage", "CA2255:The \'ModuleInitializer\' attribute should not be used in libraries")]
  public static void Initialize()
  {
    LogFactory.Current = new TextWriterLogFactory(Console.Out, LogLevel.Trace);
    ProfilerFactory.Current = new SamplingProfilerFactory(new InMemoryProfilerSampler());
  }

  private static IGameHost? _host;

  /// <summary>
  /// The top-level <see cref="IGameHost"/> for the game.
  /// </summary>
  public static IGameHost Host
  {
    get => _host ?? throw new InvalidOperationException($"The {nameof(IGameHost)} has not been set.");
    set => _host = value;
  }

  /// <summary>
  /// The top-level <see cref="IEventBus"/> for the game.
  /// </summary>
  public static IEventBus Events => Host.Events;

  /// <summary>
  /// The top-level <see cref="IAssetProvider"/> for the game.
  /// </summary>
  public static AssetManager Assets => Host.Assets;

  /// <summary>
  /// The top-level <see cref="IServiceProvider"/> for the game.
  /// </summary>
  public static IServiceProvider Services => Host.Services;

  /// <summary>
  /// Starts a game on the given platform.
  /// </summary>
  public static void Start(GameConfiguration configuration)
  {
    var bootTime = TimeStamp.Now;

    Log.Trace("Creating platform");

    using var platform = configuration.Platform.BuildHost(configuration.Host);
    using var host = configuration.Host;

    Host = host;

    try
    {
      Log.Trace("Initializing");

      host.Initialize(configuration);

      var clock = new DeltaTimeClock();
      var startTime = TimeStamp.Now;

      Log.Trace($"Startup took {TimeStamp.Now - bootTime:c}");
      Log.Trace("Starting main loop");

      while (!platform.IsClosing && !host.IsClosing)
      {
        var time = new GameTime
        {
          DeltaTime = clock.Tick(),
          TotalTime = TimeStamp.Now - startTime
        };

        platform.BeginFrame(time.DeltaTime);

        host.Tick(time);

        platform.EndFrame(time.DeltaTime);
      }

      Log.Trace("Shutting down");
    }
    finally
    {
      Host = null!;
    }
  }

  /// <summary>
  /// Exits the current game.
  /// </summary>
  public static void Exit()
  {
    Host.Exit();
  }
}
