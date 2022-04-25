var platform = new HeadlessPlatform();

Game.Start(platform, context =>
{
  var log = LogFactory.GetLog<HeadlessPlatform>();

  var frameCounter = new FrameCounter();
  var fpsTimer = new IntervalTimer(1.Seconds());

  context.ExecuteVariableStep(time =>
  {
    if (fpsTimer.Tick(time.DeltaTime))
    {
      log.Trace($"Ticks per second: {frameCounter.TicksPerSecond:F}");
    }

    frameCounter.Tick(time.DeltaTime);
  });

  return Task.CompletedTask;
});
