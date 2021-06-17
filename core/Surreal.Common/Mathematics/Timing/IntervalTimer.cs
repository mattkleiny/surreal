using System;
using System.Diagnostics;

namespace Surreal.Mathematics.Timing {
  public struct IntervalTimer {
    private readonly TimeSpan interval;
    private          int      lastUpdate;

    public IntervalTimer(TimeSpan interval) {
      Debug.Assert(interval.Ticks > 0, "interval.Ticks > 0");

      this.interval = interval;
      lastUpdate    = Environment.TickCount;
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