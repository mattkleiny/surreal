using System;
using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Diagnostics {
  public sealed class FrameCounter {
    private readonly RingBuffer<TimeSpan> samples;

    public FrameCounter(int sampleCount = 100) {
      samples = new RingBuffer<TimeSpan>(sampleCount);
    }

    public double TotalFrameTime  => samples.FastSum().TotalSeconds;
    public double FramesPerSecond => samples.Count / TotalFrameTime;

    public void Tick(DeltaTime deltaTime) {
      samples.Add(deltaTime);
    }
  }
}