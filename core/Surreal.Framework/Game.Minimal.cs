using System.Runtime;
using Surreal.Internal;
using Surreal.Timing;

namespace Surreal;

/// <summary>Context for per-frame game loop updates.</summary>
public sealed record GameContext(IPlatformHost Host, CancellationToken CancellationToken)
{
  public bool IsClosing { get; private set; } = false;

  /// <summary>Exits the game at the end of the frame.</summary>
  public void Exit()
  {
    IsClosing = true;
  }
}

public abstract partial class Game
{
  public delegate GameLoopDelegate GameSetupDelegate(IServiceRegistry services);
  public delegate ValueTask        GameLoopDelegate(GameContext context, GameTime time);

  /// <summary>Bootstraps a delegate-based game with the given <see cref="platform"/>.</summary>
  public static async ValueTask StartAsync(IPlatform platform, GameSetupDelegate gameSetup, CancellationToken cancellationToken = default)
  {
    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

    using var              host     = platform.BuildHost();
    using IServiceRegistry services = new ServiceRegistry();

    services.AddSingleton(host);
    services.AddModule(host.Services);

    var stopwatch = new Chronometer();
    var startTime = TimeStamp.Now;
    var gameLoop  = gameSetup(services);
    var context   = new GameContext(host, cancellationToken);

    while (!host.IsClosing && !context.IsClosing && !cancellationToken.IsCancellationRequested)
    {
      var deltaTime = stopwatch.Tick();
      var totalTime = TimeStamp.Now - startTime;

      host.Tick(deltaTime);

      var gameTime = new GameTime(
        DeltaTime: deltaTime,
        TotalTime: totalTime,
        IsRunningSlowly: deltaTime > 32.Milliseconds()
      );

      await gameLoop(context, gameTime);
      await host.Dispatcher.Yield();
    }
  }
}
