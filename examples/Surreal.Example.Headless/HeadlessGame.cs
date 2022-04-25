var platform = new HeadlessPlatform();

await Game.Start(platform, context =>
{
  var log = LogFactory.GetLog<HeadlessPlatform>();

  var frameCounter = new FrameCounter();
  var fpsTimer = new IntervalTimer(1.Seconds());

  context.Execute(time =>
  {
    if (fpsTimer.Tick(time.DeltaTime))
    {
      log.Trace($"Ticks per second: {frameCounter.TicksPerSecond:F}");
    }

    frameCounter.Tick(time.DeltaTime);
  });

  return ValueTask.CompletedTask;
});
