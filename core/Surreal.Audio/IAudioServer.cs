using Surreal.Audio.Clips;

namespace Surreal.Audio;

/// <summary>
/// Represents the audio subsystem.
/// </summary>
public interface IAudioServer
{
  // audio clips
  AudioHandle CreateAudioClip();
  void WriteAudioClipData<T>(AudioHandle clip, AudioSampleRate sampleRate, ReadOnlySpan<T> data) where T : unmanaged;
  void DeleteAudioClip(AudioHandle clip);

  // audio sources
  AudioHandle CreateAudioSource();
  bool IsAudioSourcePlaying(AudioHandle handle);
  void PlayAudioSource(AudioHandle source, AudioHandle clip);
  void StopAudioSource(AudioHandle source);
  void SetAudioSourceVolume(AudioHandle source, float value);
  void SetAudioSourceLooping(AudioHandle source, bool value);
  void DeleteAudioSource(AudioHandle source);
}
