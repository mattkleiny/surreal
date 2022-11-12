namespace Surreal.Timing;

/// <summary>A stop watch using a precision <see cref="TimeStamp" />.</summary>
public sealed class Chronometer
{
  private TimeStamp _lastTime = TimeStamp.Now;

  public TimeSpan TargetDeltaTime { get; } = 16.Milliseconds();
  public TimeSpan MaxDeltaTime { get; } = (16 * 10).Milliseconds();

  public TimeDelta Tick()
  {
    var now = TimeStamp.Now;
    var delta = now - _lastTime;

    if (delta > MaxDeltaTime)
    {
      delta = TargetDeltaTime;
    }

    _lastTime = now;

    return new TimeDelta(delta);
  }
}



