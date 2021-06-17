using System;
using System.Diagnostics;

namespace Surreal.Mathematics.Timing {
  [DebuggerDisplay("{TimeSpan.TotalMilliseconds}ms")]
  public readonly struct DeltaTime : IEquatable<DeltaTime> {
    public DeltaTime(TimeSpan timeSpan)
        : this((float)timeSpan.TotalSeconds) {
    }

    public DeltaTime(float seconds) {
      Seconds = seconds;
    }

    public readonly float    Seconds;
    public          TimeSpan TimeSpan => TimeSpan.FromSeconds(Seconds);

    public override string ToString() => TimeSpan.TotalMilliseconds.ToString("F2") + "ms";

    public          bool Equals(DeltaTime other) => Seconds.Equals(other.Seconds);
    public override bool Equals(object? obj)     => obj is DeltaTime other && Equals(other);

    public override int GetHashCode() => Seconds.GetHashCode();

    public static bool operator ==(DeltaTime left, DeltaTime right) => left.Equals(right);
    public static bool operator !=(DeltaTime left, DeltaTime right) => !left.Equals(right);

    public static implicit operator DeltaTime(TimeSpan timeSpan)  => new(timeSpan);
    public static implicit operator DeltaTime(float seconds)      => new(seconds);
    public static implicit operator TimeSpan(DeltaTime deltaTime) => deltaTime.TimeSpan;
    public static implicit operator float(DeltaTime deltaTime)    => deltaTime.Seconds;
  }
}