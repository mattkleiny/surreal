using System;
using System.Runtime.InteropServices;

namespace Surreal.Timing
{
  /// <summary>A precision timestamp evaluated using native APIs.</summary>
  public readonly record struct TimeStamp(ulong Ticks) : IComparable<TimeStamp>, IComparable
  {
    public static TimeStamp Min => new(ulong.MinValue);
    public static TimeStamp Max => new(ulong.MaxValue);

    public static TimeStamp Now
    {
      get
      {
        QueryPerformanceCounter(out var ticks);

        return new TimeStamp((ulong)ticks);
      }
    }

    public TimeSpan ElapsedTime => Now - this;

    public int CompareTo(TimeStamp other)
    {
      return Ticks.CompareTo(other.Ticks);
    }

    public int CompareTo(object? obj)
    {
      if (ReferenceEquals(null, obj)) return 1;

      return obj is TimeStamp other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TimeStamp)}");
    }

    public static bool operator <(TimeStamp left, TimeStamp right)  => left.CompareTo(right) < 0;
    public static bool operator >(TimeStamp left, TimeStamp right)  => left.CompareTo(right) > 0;
    public static bool operator <=(TimeStamp left, TimeStamp right) => left.CompareTo(right) <= 0;
    public static bool operator >=(TimeStamp left, TimeStamp right) => left.CompareTo(right) >= 0;

    public static TimeSpan operator +(TimeStamp left, TimeStamp right) => new(checked((long)(left.Ticks + right.Ticks)));
    public static TimeSpan operator -(TimeStamp left, TimeStamp right) => new(checked((long)(left.Ticks - right.Ticks)));

    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long value);
  }
}
