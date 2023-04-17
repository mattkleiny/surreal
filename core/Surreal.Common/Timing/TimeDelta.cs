namespace Surreal.Timing;

/// <summary>
/// Represents a change in time, most commonly represented as seconds.
/// </summary>
public readonly record struct TimeDelta(float Seconds)
{
  public TimeDelta(TimeSpan timeSpan)
    : this((float)timeSpan.TotalSeconds)
  {
  }

  public TimeSpan TimeSpan => TimeSpan.FromSeconds(Seconds);

  public override string ToString()
  {
    return $"{TimeSpan.TotalMilliseconds:F}ms";
  }

  public static implicit operator TimeDelta(TimeSpan timeSpan)
  {
    return new TimeDelta(timeSpan);
  }

  public static implicit operator TimeDelta(float seconds)
  {
    return new TimeDelta(seconds);
  }

  public static implicit operator TimeDelta(double seconds)
  {
    return new TimeDelta((float)seconds);
  }

  public static implicit operator TimeSpan(TimeDelta deltaTime)
  {
    return deltaTime.TimeSpan;
  }

  public static implicit operator float(TimeDelta deltaTime)
  {
    return deltaTime.Seconds;
  }

  public static implicit operator double(TimeDelta deltaTime)
  {
    return deltaTime.Seconds;
  }
}
