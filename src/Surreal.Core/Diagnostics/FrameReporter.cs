using System;
using Surreal.Diagnostics.Logging;
using Surreal.Timing;

namespace Surreal.Diagnostics
{
  public sealed class FrameReporter
  {
    private readonly ILog          log;
    private readonly IntervalTimer timer;
    private readonly FrameCounter  counter;

    public FrameReporter(ILog log)
      : this(log, 5.Seconds())
    {
    }

    public FrameReporter(ILog log, TimeSpan interval, int sampleCount = 100)
    {
      this.log = log;

      timer   = new IntervalTimer(interval);
      counter = new FrameCounter(sampleCount);
    }

    public void Tick(DeltaTime deltaTime)
    {
      if (!log.IsLevelEnabled(LogLevel.Trace)) return;

      if (timer.Tick())
      {
        log.Trace($"Frames per second: {counter.FramesPerSecond:F}");
      }

      counter.Tick(deltaTime);
    }
  }
}
