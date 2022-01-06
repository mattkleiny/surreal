namespace Headless;

public sealed class HeadlessGame : PrototypeGame
{
  private static readonly ILog Log = LogFactory.GetLog<HeadlessGame>();

  private readonly FrameCounter  frameCounter = new();
  private          IntervalTimer fpsTimer     = new(1.Seconds());

  public static Task Main() => StartAsync<HeadlessGame>(new Configuration
  {
    Platform = new HeadlessPlatform(),
  });

  protected override void Draw(GameTime time)
  {
    base.Draw(time);

    if (fpsTimer.Tick(time.DeltaTime))
    {
      Log.Trace($"Frames per second: {frameCounter.FramesPerSecond:F}");
    }

    frameCounter.Tick(time.DeltaTime);
  }
}
