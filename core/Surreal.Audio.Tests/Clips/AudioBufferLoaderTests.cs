using Surreal.Assets;

namespace Surreal.Audio.Clips;

public class AudioBufferLoaderTests
{
  [Test]
  [TestCase("Assets/audio/test.wav")]
  public async Task it_should_load_an_audio_buffer(string path)
  {
    using var manager = new AssetManager();

    manager.AddLoader(new AudioBufferLoader());

    var buffer = await manager.LoadAssetAsync<AudioBuffer>(path);

    Assert.That(buffer.Rate.Frequency, Is.GreaterThan(0));
    Assert.That(buffer.Data.Length, Is.GreaterThan(0));
  }
}
