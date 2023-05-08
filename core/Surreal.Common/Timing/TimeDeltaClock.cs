namespace Surreal.Timing;

/// <summary>
/// A stop watch using a precision <see cref="TimeStamp" />.
/// </summary>
public sealed class TimeDeltaClock
{
  private TimeStamp _lastTime = TimeStamp.Now;

  public TimeSpan TargetDeltaTime { get; } = TimeSpan.FromMilliseconds(16);
  public TimeSpan MaxDeltaTime { get; } = TimeSpan.FromMilliseconds(16 * 10);

  /// <summary>
  /// Advances the clock and returns the delta time.
  /// </summary>
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
