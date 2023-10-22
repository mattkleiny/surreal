using Surreal.Memory;

namespace Surreal.Audio.Clips;

public class AudioBufferTests
{
  [Test]
  public void it_should_allocate_buffer_data()
  {
    using var buffer = new AudioBuffer(TimeSpan.FromSeconds(1), AudioSampleRate.Standard);

    buffer.Size.Should().BeGreaterThan(Size.FromKilobytes(1));
  }
}
