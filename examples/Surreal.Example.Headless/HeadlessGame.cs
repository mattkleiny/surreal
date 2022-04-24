namespace Headless;

public sealed class HeadlessGame : PrototypeGame
{
  private static readonly ILog Log = LogFactory.GetLog<HeadlessGame>();

  private readonly FrameCounter frameCounter = new();
  private IntervalTimer fpsTimer = new(1.Seconds());

  public static void Main() => Start<HeadlessGame>(new Configuration
  {
    Platform = new HeadlessPlatform(),
  });

  protected override void OnUpdate(GameTime time)
  {
    base.OnUpdate(time);

    if (fpsTimer.Tick(time.DeltaTime))
    {
      Log.Trace($"Ticks per second: {frameCounter.TicksPerSecond:F}");
    }

    frameCounter.Tick(time.DeltaTime);
  }
}
