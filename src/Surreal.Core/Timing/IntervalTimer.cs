using System;

namespace Surreal.Timing {
  public sealed class IntervalTimer {
    private readonly TimeSpan interval;

    private int lastUpdate = Environment.TickCount;

    public IntervalTimer(TimeSpan interval) {
      this.interval = interval;
    }

    public bool Tick() {
      var now   = Environment.TickCount;
      var delta = (now - lastUpdate).Milliseconds();

      if (delta > interval) {
        lastUpdate = now;
        return true;
      }

      return false;
    }
  }
}