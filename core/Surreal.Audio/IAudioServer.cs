using Surreal.Audio.Clips;
using Surreal.Audio.Playback;

namespace Surreal.Audio;

/// <summary>An opaque handle to an audio resource in the underling <see cref="IAudioServer"/> implementation.</summary>
public readonly record struct AudioId(uint Id)
{
  public AudioId(int id)
    : this((uint) id)
  {
  }

  public static implicit operator uint(AudioId id) => id.Id;
  public static implicit operator int(AudioId id)  => (int) id.Id;
}

/// <summary>Represents the audio subsystem.</summary>
public interface IAudioServer
{
  /// <summary>The top-level master volume for the entire device.</summary>
  float MasterVolume { get; set; }

  AudioClip   CreateAudioClip(IAudioData data);
  AudioSource CreateAudioSource();
}
