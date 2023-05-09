namespace Surreal;

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
  /// The render function for the game.
  /// </summary>
  public Action<GameTime, IGraphicsServer> Tick { get; init; } = (_, graphics) =>
  {
    graphics.Backend.ClearColorBuffer(ColorF.Black);
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

    var graphics = registry.GetRequiredService<IGraphicsServer>();

    var clock = new TimeDeltaClock();
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

      configuration.Tick.Invoke(gameTime, graphics);

      host.EndFrame(deltaTime);
    }
  }
}
