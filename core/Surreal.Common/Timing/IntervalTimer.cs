namespace Surreal.Timing;

/// <summary>A stack-allocated interval timer.</summary>
public struct IntervalTimer
{
  private readonly TimeSpan interval;
  private float accumulator;

  public IntervalTimer(TimeSpan interval)
  {
    Debug.Assert(interval.Ticks > 0, "frequency.Ticks > 0");

    this.interval = interval;
    accumulator = 0f;
  }

  public bool Tick(DeltaTime deltaTime)
  {
    accumulator += deltaTime;

    if (accumulator >= interval.TotalSeconds)
    {
      accumulator = 0f;
      return true;
    }

    return false;
  }

  public void Reset()
  {
    accumulator = 0f;
  }
}
