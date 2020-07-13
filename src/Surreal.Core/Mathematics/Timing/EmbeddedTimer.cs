using System;
using System.Diagnostics;

namespace Surreal.Mathematics.Timing {
  [DebuggerDisplay("Embedded timer every {frequency} (currently {accumulator}s)")]
  public struct EmbeddedTimer {
    private readonly TimeSpan frequency;
    private          float    accumulator;

    public EmbeddedTimer(TimeSpan frequency) {
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