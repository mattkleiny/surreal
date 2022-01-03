using System.Diagnostics;

namespace Surreal.Timing;

/// <summary>Wraps delta-time for passing through the callstack.</summary>
[DebuggerDisplay("{TimeSpan.TotalMilliseconds}ms")]
public readonly record struct DeltaTime(float Seconds)
{
  public DeltaTime(TimeSpan timeSpan)
    : this((float) timeSpan.TotalSeconds)
  {
  }

  public TimeSpan TimeSpan => TimeSpan.FromSeconds(Seconds);

  public static implicit operator DeltaTime(TimeSpan timeSpan)  => new(timeSpan);
  public static implicit operator DeltaTime(float seconds)      => new(seconds);
  public static implicit operator DeltaTime(double seconds)     => new((float) seconds);
  public static implicit operator TimeSpan(DeltaTime deltaTime) => deltaTime.TimeSpan;
  public static implicit operator float(DeltaTime deltaTime)    => deltaTime.Seconds;
  public static implicit operator double(DeltaTime deltaTime)   => deltaTime.Seconds;
}
