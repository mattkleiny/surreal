using System.Runtime;
using Surreal.Internal;
using Surreal.Timing;

namespace Surreal;

/// <summary>Context for per-frame game loop updates.</summary>
public sealed record GameContext(IPlatformHost Host)
{
  public bool IsClosing { get; private set; } = false;

  public void Exit()
  {
    IsClosing = true;
  }
}

public delegate GameLoopDelegate GameSetupDelegate(IServiceRegistry services);
public delegate ValueTask        GameLoopDelegate(GameContext context, GameTime time);

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
    var context   = new GameContext(host);

    while (!host.IsClosing && !context.IsClosing)
    {
      var deltaTime = stopwatch.Tick();
      var totalTime = TimeStamp.Now - startTime;

      host.Tick(deltaTime);

      var gameTime = new GameTime(
        DeltaTime: deltaTime,
        TotalTime: totalTime,
        IsRunningSlowly: deltaTime > 32.Milliseconds()
      );

      gameLoop(context, gameTime);

      Thread.Yield();
    }
  }
}
