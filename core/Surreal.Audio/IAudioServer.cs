using Surreal.Audio.Clips;
using Surreal.Audio.Playback;

namespace Surreal.Audio;

/// <summary>An opaque handle to a resource in the underling <see cref="IAudioServer"/> implementation.</summary>
public readonly record struct AudioHandle(uint Id)
{
  public AudioHandle(int id)
    : this((uint) id)
  {
  }

  public static implicit operator uint(AudioHandle handle) => handle.Id;
  public static implicit operator int(AudioHandle handle)  => (int) handle.Id;
}

/// <summary>Represents the audio subsystem.</summary>
public interface IAudioServer
{
  /// <summary>The top-level master volume for the entire device.</summary>
  float MasterVolume { get; set; }

  AudioClip   CreateAudioClip(IAudioData data);
  AudioSource CreateAudioSource();
}
