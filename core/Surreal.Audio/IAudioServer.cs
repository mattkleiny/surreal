using Surreal.Audio.Clips;

namespace Surreal.Audio;

/// <summary>An opaque handle to a resource in the underling <see cref="IAudioServer"/> implementation.</summary>
public readonly record struct AudioHandle(nint Id)
{
  public static implicit operator nint(AudioHandle handle) => handle.Id;
  public static implicit operator int(AudioHandle handle) => (int) handle.Id;
  public static implicit operator uint(AudioHandle handle) => (uint) handle.Id;
}

/// <summary>Represents the audio subsystem.</summary>
public interface IAudioServer
{
  // audio clips
  AudioHandle CreateAudioClip();
  void DeleteAudioClip(AudioHandle clip);
  void WriteAudioClipData<T>(AudioHandle clip, AudioSampleRate sampleRate, ReadOnlySpan<T> data) where T : unmanaged;

  // audio sources
  AudioHandle CreateAudioSource();
  bool IsAudioSourcePlaying(AudioHandle handle);
  void PlayAudioSource(AudioHandle source, AudioHandle clip);
  void StopAudioSource(AudioHandle source);
  void SetAudioSourceVolume(AudioHandle source, float value);
  void SetAudioSourceLooping(AudioHandle source, bool value);
  void DeleteAudioSource(AudioHandle source);
}
