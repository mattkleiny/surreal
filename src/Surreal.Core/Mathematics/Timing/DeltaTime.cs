using System;
using System.Diagnostics;

namespace Surreal.Mathematics.Timing {
  [DebuggerDisplay("{TimeSpan.TotalMilliseconds}ms")]
  public readonly struct DeltaTime {
    public DeltaTime(float seconds) {
      TimeSpan = TimeSpan.FromSeconds(seconds);
    }

    public DeltaTime(TimeSpan timeSpan) {
      TimeSpan = timeSpan;
    }

    public readonly TimeSpan TimeSpan;

    public static implicit operator DeltaTime(TimeSpan span)      => new DeltaTime(span);
    public static implicit operator TimeSpan(DeltaTime deltaTime) => deltaTime.TimeSpan;
    public static implicit operator float(DeltaTime deltaTime)    => (float) deltaTime.TimeSpan.TotalSeconds;

    public override string ToString() => TimeSpan.TotalMilliseconds.ToString("F2") + "ms";
  }
}