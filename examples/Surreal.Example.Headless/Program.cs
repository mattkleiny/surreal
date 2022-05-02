var platform = new HeadlessPlatform();

Game.Start(platform, game =>
{
  // ReSharper disable AccessToDisposedClosure

  var log = LogFactory.GetLog<HeadlessPlatform>();

  var frameCounter = new FrameCounter();
  var fpsTimer = new IntervalTimer(1.Seconds());

  game.ExecuteVariableStep(time =>
  {
    if (fpsTimer.Tick(time.DeltaTime))
    {
      log.Trace($"Ticks per second: {frameCounter.TicksPerSecond:F}");
    }

    frameCounter.Tick(time.DeltaTime);
  });

  return Task.CompletedTask;
});
