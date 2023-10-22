using Surreal.Audio.Clips;

namespace Surreal.Audio;

/// <summary>
/// An abstraction over the different types of audio backends available.
/// </summary>
public interface IAudioBackend
{
  /// <summary>
  /// A no-op <see cref="IAudioBackend" /> for headless environments and testing.
  /// </summary>
  static IAudioBackend Headless { get; } = new HeadlessAudioBackend();

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
