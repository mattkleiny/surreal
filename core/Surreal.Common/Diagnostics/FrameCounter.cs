using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Diagnostics;

/// <summary>
/// A utility which counts frames per second.
/// </summary>
public sealed class FrameCounter(int sampleCount = 100)
{
  private readonly RingBuffer<TimeSpan> _samples = new(sampleCount);

  public double TotalFrameTime => _samples.FastSum().TotalSeconds;
  public double TicksPerSecond => _samples.Count / TotalFrameTime;

  public void Tick(TimeDelta deltaTime)
  {
    _samples.Add(deltaTime);
  }
}
