namespace Surreal;

/// <summary>
/// A callback for the main game loop.
/// </summary>
public delegate void TickDelegate(GameTime time, IGraphicsContext graphics);

/// <summary>
/// A timing snapshot for the main game loop.
/// </summary>
public readonly record struct GameTime
{
  public required float DeltaTime { get; init; }
  public required float TotalTime { get; init; }
}

/// <summary>
/// Configuration for the game.
/// </summary>
public sealed record GameConfiguration
{
  /// <summary>
  /// The platform to use for the game.
  /// </summary>
  public required IPlatform Platform { get; init; }

  /// <summary>
  /// The <see cref="TickDelegate"/> for the game.
  /// </summary>
  public TickDelegate Tick { get; init; } = (_, graphics) =>
  {
    graphics.Backend.ClearColorBuffer(Color.Black);
    graphics.Backend.FlushToDevice();
  };
}

/// <summary>
/// Bootstraps the game.
/// </summary>
public static class Game
{
  /// <summary>
  /// Starts a game on the given platform.
  /// </summary>
  public static void Start(GameConfiguration configuration)
  {
    using var host = configuration.Platform.BuildHost();
    using var registry = new ServiceRegistry();

    host.RegisterServices(registry);

    using var graphics = new GraphicsContext(registry.GetServiceOrThrow<IGraphicsBackend>());

    var clock = new DeltaTimeClock();
    var startTime = TimeStamp.Now;

    while (!host.IsClosing)
    {
      var deltaTime = clock.Tick();
      var totalTime = TimeStamp.Now - startTime;

      var gameTime = new GameTime
      {
        DeltaTime = deltaTime,
        TotalTime = (float)totalTime.TotalSeconds
      };

      host.BeginFrame(deltaTime);

      configuration.Tick(gameTime, graphics);

      host.EndFrame(deltaTime);
    }
  }
}
