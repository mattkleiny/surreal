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

  public void Reset()
  {
    _accumulator = 0f;
  }
}
