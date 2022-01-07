using System.Runtime;
using Surreal.Fibers;
using Surreal.Internal;
using Surreal.Timing;

namespace Surreal;

/// <summary>Context for per-frame game loop updates.</summary>
public readonly record struct GameContext(IPlatformHost Host, GameTime GameTime);

public delegate GameLoopDelegate GameSetupDelegate(IServiceRegistry services);
public delegate FiberTask        GameLoopDelegate(GameContext context);

public abstract partial class Game
{
  /// <summary>Bootstraps a delegate-based game with the given <see cref="platform"/>.</summary>
  public static void Start(IPlatform platform, GameSetupDelegate gameSetup)
  {
    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

    using var              host     = platform.BuildHost();
    using IServiceRegistry services = new ServiceRegistry();

    services.AddSingleton(host);
    services.AddModule(host.Services);

    var stopwatch = new Chronometer();
    var startTime = TimeStamp.Now;
    var gameLoop  = gameSetup(services);

    while (!host.IsClosing)
    {
      var deltaTime = stopwatch.Tick();
      var totalTime = TimeStamp.Now - startTime;

      host.Tick(deltaTime);

      var gameTime = new GameTime(
        DeltaTime: deltaTime,
        TotalTime: totalTime,
        IsRunningSlowly: deltaTime > 32.Milliseconds()
      );

      gameLoop(new GameContext(host, gameTime));

      Thread.Yield();
    }
  }
}
