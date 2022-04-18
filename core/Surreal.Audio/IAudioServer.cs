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
  void DeleteAudioClip(AudioHandle handle);
  void WriteAudioClipData<T>(AudioHandle handle, AudioSampleRate sampleRate, ReadOnlySpan<T> buffer) where T : unmanaged;

  // audio sources
  AudioHandle CreateAudioSource();
  void DeleteAudioSource(AudioHandle handle);
}
