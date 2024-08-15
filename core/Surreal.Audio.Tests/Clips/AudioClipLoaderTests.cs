using Surreal.Assets;

namespace Surreal.Audio.Clips;

public class AudioClipLoaderTests
{
  [Test]
  [TestCase("Assets/External/audio/test.wav")]
  public async Task it_should_load_an_audio_buffer(string path)
  {
    using var manager = new AssetManager();

    manager.AddLoader(new AudioClipLoader(IAudioDevice.Null));

    var buffer = await manager.LoadAsync<AudioClip>(path);

    buffer.Value.Rate.Frequency.Should().BeGreaterThan(0);
    buffer.Value.Duration.Should().BeGreaterThan(TimeSpan.Zero);
  }
}
