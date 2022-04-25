﻿namespace Surreal.Timing;

/// <summary>Represents a change in time, most commonly represented as seconds.</summary>
public readonly record struct TimeDelta(float Seconds)
{
  public TimeDelta(TimeSpan timeSpan)
    : this((float) timeSpan.TotalSeconds)
  {
  }

  public TimeSpan TimeSpan => TimeSpan.FromSeconds(Seconds);

  public override string ToString()
  {
    return $"{TimeSpan.TotalMilliseconds:F}ms";
  }

  public static implicit operator TimeDelta(TimeSpan timeSpan) => new(timeSpan);
  public static implicit operator TimeDelta(float seconds) => new(seconds);
  public static implicit operator TimeDelta(double seconds) => new((float) seconds);
  public static implicit operator TimeSpan(TimeDelta deltaTime) => deltaTime.TimeSpan;
  public static implicit operator float(TimeDelta deltaTime) => deltaTime.Seconds;
  public static implicit operator double(TimeDelta deltaTime) => deltaTime.Seconds;
}