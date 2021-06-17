using System;
using Surreal.Diagnostics.Logging;
using Surreal.Mathematics.Timing;

namespace Surreal.Diagnostics {
  public sealed class FrameReporter {
    private readonly ILog          log;
    private readonly FrameCounter  counter;
    private          IntervalTimer timer;

    public FrameReporter(ILog log)
        : this(log, 5.Seconds()) {
    }

    public FrameReporter(ILog log, TimeSpan interval, int sampleCount = 100) {
      this.log = log;

      timer   = new IntervalTimer(interval);
      counter = new FrameCounter(sampleCount);
    }

    public void Tick(DeltaTime deltaTime) {
      if (!log.IsLevelEnabled(LogLevel.Trace)) return;

      if (timer.Tick()) {
        log.Trace($"Frames per second: {counter.FramesPerSecond.ToString("F")}");
      }

      counter.Tick(deltaTime);
    }
  }
}