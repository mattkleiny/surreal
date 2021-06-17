using System;
using System.Diagnostics;

namespace Surreal.Timing {
  public struct GlobalTimer {
    private readonly TimeSpan  interval;
    private          TimeStamp lastUpdate;

    public GlobalTimer(TimeSpan interval) {
      Debug.Assert(interval.Ticks > 0, "interval.Ticks > 0");

      this.interval = interval;
      lastUpdate    = TimeStamp.Now;
    }

    public bool Tick() {
      var now   = TimeStamp.Now;
      var delta = now - lastUpdate;

      if (delta > interval) {
        lastUpdate = now;
        return true;
      }

      return false;
    }
    
    public void Reset() {
      lastUpdate = TimeStamp.Now;
    }
  }
}