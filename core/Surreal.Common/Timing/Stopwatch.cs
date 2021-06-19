using System;

namespace Surreal.Timing
{
  public sealed class Stopwatch
  {
    private TimeStamp lastTime = TimeStamp.Now;

    public TimeSpan TargetDeltaTime { get; } = 16.Milliseconds();
    public TimeSpan MaxDeltaTime    { get; } = (16 * 10).Milliseconds();

    public DeltaTime Tick()
    {
      var now   = TimeStamp.Now;
      var delta = now - lastTime;

      if (delta > MaxDeltaTime)
      {
        delta = TargetDeltaTime;
      }

      lastTime = now;

      return new DeltaTime(delta);
    }
  }
}