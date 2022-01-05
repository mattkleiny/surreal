﻿using NUnit.Framework;
using Surreal.Audio.Clips;
using Surreal.Memory;
using Surreal.Timing;

namespace Surreal.Clips;

public class AudioBufferTests
{
  [Test]
  public void it_should_allocate_buffer_data()
  {
    using var buffer = new AudioBuffer(1.Seconds(), AudioSampleRate.Standard);

    Assert.That(buffer.Size, Is.GreaterThan(1.Kilobytes()));
  }
}
