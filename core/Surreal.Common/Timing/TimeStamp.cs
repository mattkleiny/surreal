namespace Surreal.Timing;

/// <summary>
/// A precision timestamp evaluated using native APIs.
/// </summary>
public readonly record struct TimeStamp(long Ticks) : IComparable<TimeStamp>
{
  public static TimeStamp Min => new(long.MinValue);
  public static TimeStamp Max => new(long.MaxValue);
  public static TimeStamp Now => new(Stopwatch.GetTimestamp());

  public TimeSpan ElapsedTime => Now - this;

  public int CompareTo(TimeStamp other) => Ticks.CompareTo(other.Ticks);

  public static bool operator <(TimeStamp left, TimeStamp right) => left.Ticks < right.Ticks;
  public static bool operator >(TimeStamp left, TimeStamp right) => left.Ticks > right.Ticks;
  public static bool operator <=(TimeStamp left, TimeStamp right) => left.Ticks <= right.Ticks;
  public static bool operator >=(TimeStamp left, TimeStamp right) => left.Ticks >= right.Ticks;

  public static TimeSpan operator +(TimeStamp left, TimeStamp right) => new(checked(left.Ticks + right.Ticks));
  public static TimeSpan operator -(TimeStamp left, TimeStamp right) => new(checked(left.Ticks - right.Ticks));
}
