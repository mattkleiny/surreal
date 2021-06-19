using Surreal;
using Surreal.Diagnostics;
using Surreal.Diagnostics.Logging;
using Surreal.Framework;
using Surreal.Platform;
using Surreal.Timing;

namespace Headless
{
  public sealed class Game : GameJam<Game>
  {
    private static readonly ILog Log = LogFactory.GetLog<Game>();

    private readonly FpsCounter fpsCounter = new();
    private          Timer      fpsTimer   = new(1.Seconds());

    public static void Main() => Start<Game>(new()
    {
      Platform = new HeadlessPlatform(),
    });

    protected override void Draw(GameTime time)
    {
      base.Draw(time);

      if (fpsTimer.Tick(time.DeltaTime))
      {
        Log.Trace($"Frames per second: {fpsCounter.FramesPerSecond.ToString("F")}");
      }

      fpsCounter.Tick(time.DeltaTime);
    }
  }
}