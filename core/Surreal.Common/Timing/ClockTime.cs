using System.Runtime.CompilerServices;

namespace Surreal.Timing;

/// <summary>A range of <see cref="ClockTime" />s for scheduling.</summary>
public readonly record struct ClockTimeRange(ClockTime Start, ClockTime End)
{
  public static ClockTimeRange Nothing => new(default, default);
  public static ClockTimeRange EntireDay => new(ClockTime.MinValue, ClockTime.MaxValue);

  public bool Contains(ClockTime time)
  {
    return time >= Start && time <= End;
  }
}

/// <summary>Represents a discrete time in a 24 hour clock with hours, minutes and seconds.</summary>
public readonly record struct ClockTime(int Ticks) : IComparable<ClockTime>
{
  private const int TicksPerSecond = 1;
  private const int TicksPerMinute = TicksPerSecond * 60;
  private const int TicksPerHour = TicksPerMinute * 60;

  public ClockTime(DateTime dateTime)
    : this(dateTime.Hour, dateTime.Minute, dateTime.Second)
  {
  }

  public ClockTime(int hour, int minute, int second)
    : this(CalculateTicks(hour, minute, second))
  {
  }

  public static ClockTime MinValue => new(00, 00, 00);
  public static ClockTime MaxValue => new(23, 59, 59);

  public int Hours => Ticks / TicksPerHour % 24;
  public int Minutes => Ticks / TicksPerMinute % 60;
  public int Seconds => Ticks / TicksPerSecond % 60;

  public int CompareTo(ClockTime other)
  {
    return Ticks.CompareTo(other.Ticks);
  }

  public static ClockTime Parse(string raw)
  {
    if (!TryParse(raw, out var time))
    {
      throw new FormatException($"Failed to parse raw clock time from {raw}");
    }

    return time;
  }

  public static bool TryParse(string raw, out ClockTime time)
  {
    time = default;

    // split into hours:minutes:seconds
    var components = raw.Split(':');
    if (components.Length != 3)
    {
      return false;
    }

    // break into components
    if (!int.TryParse(components[0], out var hours) ||
        !int.TryParse(components[1], out var minutes) ||
        !int.TryParse(components[2], out var seconds))
    {
      return false;
    }

    // validate components
    if (hours is < 0 or > 23 ||
        minutes is < 0 or > 59 ||
        seconds is < 0 or > 59)
    {
      return false;
    }

    time = new ClockTime(hours, minutes, seconds);
    return true;
  }

  public override string ToString()
  {
    return $"{Hours:00}:{Minutes:00}:{Seconds:00}";
  }

  public static bool operator <(ClockTime left, ClockTime right)
  {
    return left.Ticks < right.Ticks;
  }

  public static bool operator >(ClockTime left, ClockTime right)
  {
    return left.Ticks > right.Ticks;
  }

  public static bool operator <=(ClockTime left, ClockTime right)
  {
    return left.Ticks <= right.Ticks;
  }

  public static bool operator >=(ClockTime left, ClockTime right)
  {
    return left.Ticks >= right.Ticks;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int CalculateTicks(int hour, int minute, int second)
  {
    var totalSeconds = hour * TicksPerHour + minute * TicksPerMinute + second;

    return totalSeconds * TicksPerSecond;
  }
}



