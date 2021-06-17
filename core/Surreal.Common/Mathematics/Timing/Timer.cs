using System;
using System.Diagnostics;

namespace Surreal.Mathematics.Timing {
  public struct Timer {
    private readonly TimeSpan frequency;
    private          float    accumulator;

    public Timer(TimeSpan frequency) {
      Debug.Assert(frequency.Ticks > 0, "frequency.Ticks > 0");

      this.frequency = frequency;
      accumulator    = 0f;
    }

    public bool Tick(DeltaTime deltaTime) {
      accumulator += deltaTime;

      if (accumulator >= frequency.TotalSeconds) {
        accumulator = 0f;
        return true;
      }

      return false;
    }

    public void Reset() {
      accumulator = 0f;
    }
  }
}