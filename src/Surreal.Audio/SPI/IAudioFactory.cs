using System.Linq;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;

namespace Surreal.Audio.SPI {
  public interface IAudioFactory {
    AudioClip   CreateAudioClip(IAudioData data);
    AudioSource CreateAudioSource();

    AudioSourcePool CreateAudioSourcePool(int capacity) {
      var sources = Enumerable
          .Range(0, capacity)
          .Select(_ => CreateAudioSource());

      return new AudioSourcePool(sources);
    }
  }
}