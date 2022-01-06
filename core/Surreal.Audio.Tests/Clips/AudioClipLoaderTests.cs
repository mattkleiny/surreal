using Surreal.Assets;

namespace Surreal.Audio.Clips;

public class AudioClipLoaderTests
{
  [Test, AutoFixture]
  public async Task it_should_load_an_audio_clip(IAudioDevice device)
  {
    using var manager = new AssetManager();

    manager.AddLoader(new AudioBufferLoader());
    manager.AddLoader(new AudioClipLoader(device));

    await manager.LoadAsset<AudioClip>("Assets/audio/test.wav");

    device.Received(1).CreateAudioClip(Arg.Any<IAudioData>());
  }
}
