using Surreal.Audio.Clips;

namespace Surreal.Audio;

/// <summary>
/// A no-op <see cref="IAudioBackend" /> for headless environments and testing.
/// </summary>
internal sealed class HeadlessAudioBackend : IAudioBackend
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
    return new AudioHandle(Interlocked.Increment(ref _nextClipId));
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
    return new AudioHandle(Interlocked.Increment(ref _nextSourceId));
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
