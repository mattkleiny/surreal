namespace Surreal.Timing;

/// <summary>
/// A stack-allocated interval timer.
/// </summary>
public struct IntervalTimer
{
  private readonly TimeSpan _interval;
  private float _accumulator;

  public IntervalTimer(TimeSpan interval)
  {
    Debug.Assert(interval.Ticks > 0, "frequency.Ticks > 0");

    _interval = interval;
    _accumulator = 0f;
  }

  /// <summary>
  /// Determines if the given duration has passed on the timer.
  /// </summary>
  public readonly bool HasPassed(TimeSpan duration)
  {
    return _accumulator >= duration.TotalSeconds;
  }

  /// <summary>
  /// Advances the timer and returns true if the interval has elapsed.
  /// </summary>
  public bool Tick(DeltaTime deltaTime)
  {
    _accumulator += deltaTime;

    return _accumulator >= _interval.TotalSeconds;
  }

  /// <summary>
  /// Resets the timer.
  /// </summary>
  public void Reset()
  {
    _accumulator = 0f;
  }
}
