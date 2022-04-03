using System.Runtime;
using Surreal.Internal;
using Surreal.Timing;

namespace Surreal;

public abstract partial class Game
{
  /// <summary>Invoked to prepare a game prior to it's main loop.</summary>
  public delegate ValueTask GameSetupDelegate(GameContext context);

  /// <summary>Invoked to execute a single frame of a game's main loop.</summary>
  public delegate void GameLoopDelegate(GameTime time);

  /// <summary>Bootstraps a delegate-based game with the given <see cref="platform"/>.</summary>
  public static async ValueTask StartAsync(IPlatform platform, GameSetupDelegate gameSetup, CancellationToken cancellationToken = default)
  {
    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

    using var host = platform.BuildHost();
    using IServiceRegistry services = new ServiceRegistry();

    services.AddSingleton(host);
    services.AddModule(host.Services);

    await gameSetup(new GameContext(services, cancellationToken));
  }

  /// <summary>Context for per-frame game loop updates.</summary>
  public sealed record GameContext(IServiceRegistry Services, CancellationToken CancellationToken)
  {
    public bool IsClosing { get; private set; } = false;

    public IPlatformHost Host => Services.GetRequiredService<IPlatformHost>();

    public async ValueTask ExecuteAsync(GameLoopDelegate gameLoop)
    {
      var stopwatch = new Chronometer();
      var startTime = TimeStamp.Now;

      while (!Host.IsClosing && !IsClosing && !CancellationToken.IsCancellationRequested)
      {
        var gameTime = new GameTime(
          DeltaTime: stopwatch.Tick(),
          TotalTime: TimeStamp.Now - startTime,
          IsRunningSlowly: stopwatch.Tick() > 32.Milliseconds()
        );

        Host.Tick(gameTime.DeltaTime);
        gameLoop(gameTime);

        await Host.Dispatcher.Yield();
      }
    }

    /// <summary>Exits the game at the end of the frame.</summary>
    public void Exit()
    {
      IsClosing = true;
    }
  }
}
