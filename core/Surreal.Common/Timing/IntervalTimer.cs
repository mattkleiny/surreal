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
  /// Advances the timer and returns true if the interval has elapsed.
  /// </summary>
  public bool Tick(TimeDelta deltaTime)
  {
    _accumulator += deltaTime;

    if (_accumulator >= _interval.TotalSeconds)
    {
      _accumulator = 0f;
      return true;
    }

    return false;
  }

  /// <summary>
  /// Resets the timer.
  /// </summary>
  public void Reset()
  {
    _accumulator = 0f;
  }
}
