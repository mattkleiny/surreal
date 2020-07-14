using System.Linq;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;

namespace Surreal.Audio {
  public interface IAudioDevice {
    float MasterVolume { get; set; }

    void Play(AudioClip clip, float volume = 1f);

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