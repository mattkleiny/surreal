namespace Surreal.Timing;

/// <summary>
/// A stop watch using a precision <see cref="TimeStamp" />.
/// </summary>
public sealed class DeltaTimeClock
{
  private TimeStamp _lastTime = TimeStamp.Now;

  /// <summary>
  /// Advances the clock and returns the delta time.
  /// </summary>
  public DeltaTime Tick()
  {
    var now = TimeStamp.Now;
    var delta = now - _lastTime;

    _lastTime = now;

    return new DeltaTime(delta);
  }
}
