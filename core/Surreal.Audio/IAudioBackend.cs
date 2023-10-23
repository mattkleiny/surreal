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

  // listeners
  void SetAudioListenerGain(float gain);
  void SetAudioListenerPosition(Vector3 vector3);
  void SetAudioListenerVelocity(Vector3 vector3);

  // audio clips
  AudioHandle CreateAudioClip();
  void WriteAudioClipData<T>(AudioHandle clip, AudioSampleRate sampleRate, ReadOnlySpan<T> data) where T : unmanaged;
  void DeleteAudioClip(AudioHandle clip);

  // audio sources
  AudioHandle CreateAudioSource();
  bool IsAudioSourcePlaying(AudioHandle handle);
  void PlayAudioSource(AudioHandle source, AudioHandle clip);
  void StopAudioSource(AudioHandle source);
  void SetAudioSourcePosition(AudioHandle source, Vector3 value);
  void SetAudioSourceGain(AudioHandle source, float gain);
  void SetAudioSourceLooping(AudioHandle source, bool value);
  void SetAudioSourceDistanceFalloff(AudioHandle source, float distance);
  void DeleteAudioSource(AudioHandle source);
}
