using Surreal.Memory;
using Surreal.Timing;

namespace Surreal.Audio.Clips;

public class AudioBufferTests
{
  [Test]
  public void it_should_allocate_buffer_data()
  {
    using var buffer = new AudioBuffer(1.Seconds(), AudioSampleRate.Standard);

    buffer.Size.Should().BeGreaterThan(1.Kilobytes());
  }
}
