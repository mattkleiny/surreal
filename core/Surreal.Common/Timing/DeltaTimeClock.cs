namespace Surreal.Timing;

/// <summary>
/// A stop watch using a precision <see cref="TimeStamp" />.
/// </summary>
public sealed class DeltaTimeClock
{
  private TimeStamp _lastTime = TimeStamp.Now;

  /// <summary>
  /// The desired delta time.
  /// </summary>
  public TimeSpan TargetDeltaTime { get; } = TimeSpan.FromMilliseconds(16);

  /// <summary>
  /// The maximum delta time.
  /// </summary>
  public TimeSpan MaxDeltaTime { get; } = TimeSpan.FromMilliseconds(16 * 10);

  /// <summary>
  /// Advances the clock and returns the delta time.
  /// </summary>
  public DeltaTime Tick()
  {
    var now = TimeStamp.Now;
    var delta = now - _lastTime;

    if (delta > MaxDeltaTime)
    {
      delta = TargetDeltaTime;
    }

    _lastTime = now;

    return new DeltaTime(delta);
  }
}
