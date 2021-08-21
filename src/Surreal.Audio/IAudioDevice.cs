using Surreal.Audio.Clips;
using Surreal.Audio.Playback;

namespace Surreal.Audio
{
  /// <summary>Represents the audio subsystem.</summary>
  public interface IAudioDevice
  {
    float MasterVolume { get; set; }

    void Play(AudioClip clip, float volume = 1f);

    AudioClip       CreateAudioClip(IAudioData data);
    AudioSource     CreateAudioSource();
    AudioSourcePool CreateAudioSourcePool(int capacity) => new(this, capacity);
  }
}
