using System;
using System.Runtime.InteropServices;

namespace Surreal.Timing {
  public readonly struct TimeStamp : IEquatable<TimeStamp>, IComparable<TimeStamp>, IComparable {
    public static TimeStamp Min => new(ulong.MinValue);
    public static TimeStamp Max => new(ulong.MaxValue);

    public static TimeStamp Now {
      get {
        QueryPerformanceCounter(out var ticks);

        return new TimeStamp((ulong) ticks);
      }
    }

    private readonly ulong ticks;

    public TimeStamp(ulong ticks) {
      this.ticks = ticks;
    }

    public ulong    Ticks       => ticks;
    public TimeSpan ElapsedTime => Now - this;

    public override string ToString()    => ticks.ToString();
    public override int    GetHashCode() => ticks.GetHashCode();

    public          bool Equals(TimeStamp other) => ticks == other.ticks;
    public override bool Equals(object? obj)     => obj is TimeStamp other && Equals(other);

    public int CompareTo(TimeStamp other) => ticks.CompareTo(other.ticks);

    public int CompareTo(object? obj) {
      if (ReferenceEquals(null, obj)) return 1;
      return obj is TimeStamp other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TimeStamp)}");
    }

    public static bool operator ==(TimeStamp left, TimeStamp right) => left.Equals(right);
    public static bool operator !=(TimeStamp left, TimeStamp right) => !left.Equals(right);
    public static bool operator <(TimeStamp left, TimeStamp right)  => left.CompareTo(right) < 0;
    public static bool operator >(TimeStamp left, TimeStamp right)  => left.CompareTo(right) > 0;
    public static bool operator <=(TimeStamp left, TimeStamp right) => left.CompareTo(right) <= 0;
    public static bool operator >=(TimeStamp left, TimeStamp right) => left.CompareTo(right) >= 0;

    public static TimeSpan operator +(TimeStamp left, TimeStamp right) => new(checked((long) (left.ticks + right.ticks)));
    public static TimeSpan operator -(TimeStamp left, TimeStamp right) => new(checked((long) (left.ticks - right.ticks)));

    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long value);
  }
}