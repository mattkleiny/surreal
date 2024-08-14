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
  static IAudioBackend Null { get; } = new NullAudioBackend();

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

  /// <summary>
  /// A no-op <see cref="IAudioBackend" /> for headless environments and testing.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullAudioBackend : IAudioBackend
  {
    private int _nextClipId;
    private int _nextSourceId;

    public void SetAudioListenerGain(float gain)
    {
    }

    public void SetAudioListenerPosition(Vector3 vector3)
    {
    }

    public void SetAudioListenerVelocity(Vector3 vector3)
    {
    }

    public AudioHandle CreateAudioClip()
    {
      return AudioHandle.FromInt(Interlocked.Increment(ref _nextClipId));
    }

    public void WriteAudioClipData<T>(AudioHandle clip, AudioSampleRate sampleRate, ReadOnlySpan<T> data)
      where T : unmanaged
    {
    }

    public void DeleteAudioClip(AudioHandle clip)
    {
    }

    public AudioHandle CreateAudioSource()
    {
      return AudioHandle.FromInt(Interlocked.Increment(ref _nextSourceId));
    }

    public bool IsAudioSourcePlaying(AudioHandle handle)
    {
      return false;
    }

    public void PlayAudioSource(AudioHandle source, AudioHandle clip)
    {
    }

    public void StopAudioSource(AudioHandle source)
    {
    }

    public void SetAudioSourcePosition(AudioHandle source, Vector3 value)
    {
    }

    public void SetAudioSourceGain(AudioHandle source, float value)
    {
    }

    public void SetAudioSourceLooping(AudioHandle source, bool value)
    {
    }

    public void SetAudioSourceDistanceFalloff(AudioHandle source, float distance)
    {
    }

    public void DeleteAudioSource(AudioHandle source)
    {
    }
  }
}
