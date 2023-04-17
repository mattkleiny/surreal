using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Diagnostics;

/// <summary>
/// A utility which counts frames per second.
/// </summary>
public sealed class FrameCounter
{
  private readonly RingBuffer<TimeSpan> _samples;

  public FrameCounter(int sampleCount = 100)
  {
    _samples = new RingBuffer<TimeSpan>(sampleCount);
  }

  public double TotalFrameTime => _samples.FastSum().TotalSeconds;
  public double TicksPerSecond => _samples.Count / TotalFrameTime;

  public void Tick(TimeDelta deltaTime)
  {
    _samples.Add(deltaTime);
  }
}
