namespace Headless;

public sealed class Game : PrototypeGame
{
  private static readonly ILog Log = LogFactory.GetLog<Game>();

  private readonly FpsCounter    fpsCounter = new();
  private          IntervalTimer fpsTimer   = new(1.Seconds());

  public static Task Main() => StartAsync<Game>(new Configuration
  {
    Platform = new HeadlessPlatform(),
  });

  protected override void Draw(GameTime time)
  {
    base.Draw(time);

    if (fpsTimer.Tick(time.DeltaTime))
    {
      Log.Trace($"Frames per second: {fpsCounter.FramesPerSecond:F}");
    }

    fpsCounter.Tick(time.DeltaTime);
  }
}
