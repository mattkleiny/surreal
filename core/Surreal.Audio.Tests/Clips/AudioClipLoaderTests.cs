using Surreal.Assets;

namespace Surreal.Audio.Clips;

public class AudioClipLoaderTests
{
  [Test, AutoFixture]
  public async Task it_should_load_an_audio_clip(IAudioServer server)
  {
    using var manager = new AssetManager();

    manager.AddLoader(new AudioBufferLoader());
    manager.AddLoader(new AudioClipLoader(server));

    await manager.LoadAssetAsync<AudioClip>("Assets/audio/test.wav");

    server.Received(1).CreateAudioClip(Arg.Any<IAudioData>());
  }
}
