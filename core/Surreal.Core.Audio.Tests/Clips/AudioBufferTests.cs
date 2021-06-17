using Surreal.Mathematics.Timing;
using Xunit;

namespace Surreal.Audio.Clips {
  public class AudioBufferTests {
    [Fact]
    public void it_should_create_a_valid_audio_buffer() {
      using var buffer = new AudioBuffer(10.Seconds(), AudioSampleRate.Standard);
    }
  }
}