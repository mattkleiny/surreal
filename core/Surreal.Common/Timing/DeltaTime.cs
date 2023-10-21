namespace Surreal.Timing;

/// <summary>
/// Represents a change in time, most commonly represented as seconds.
/// </summary>
public readonly record struct DeltaTime(float Seconds)
{
  public static readonly DeltaTime Zero = new(0);
  public static readonly DeltaTime One = new(1);
  public static readonly DeltaTime OneOver60 = new(1f / 60f);

  public DeltaTime(TimeSpan timeSpan)
    : this((float)timeSpan.TotalSeconds)
  {
  }

  public TimeSpan TimeSpan => TimeSpan.FromSeconds(Seconds);

  public override string ToString()
  {
    return $"{TimeSpan.TotalMilliseconds:F}ms";
  }

  public static implicit operator DeltaTime(TimeSpan timeSpan) => new(timeSpan);
  public static implicit operator DeltaTime(float seconds) => new(seconds);
  public static implicit operator DeltaTime(double seconds) => new((float)seconds);

  public static implicit operator TimeSpan(DeltaTime deltaTime) => deltaTime.TimeSpan;
  public static implicit operator float(DeltaTime deltaTime) => deltaTime.Seconds;
  public static implicit operator double(DeltaTime deltaTime) => deltaTime.Seconds;
}
