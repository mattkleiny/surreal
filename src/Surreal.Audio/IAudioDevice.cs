using Surreal.Audio.Clips;
using Surreal.Audio.Playback;

namespace Surreal.Audio;

/// <summary>Represents the audio subsystem.</summary>
public interface IAudioDevice
{
  float MasterVolume { get; set; }

  AudioClip   CreateAudioClip(IAudioData data);
  AudioSource CreateAudioSource();
}
