using System.Linq;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;

namespace Surreal.Audio
{
  /// <summary>Represents the audio subsystem.</summary>
  public interface IAudioDevice
  {
    float MasterVolume { get; set; }

    void Play(AudioClip clip, float volume = 1f);

    AudioClip   CreateAudioClip(IAudioData data);
    AudioSource CreateAudioSource();

    AudioSourcePool CreateAudioSourcePool(int capacity)
    {
      return new(Enumerable.Range(0, capacity).Select(_ => CreateAudioSource()));
    }
  }
}
