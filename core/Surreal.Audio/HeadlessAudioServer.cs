using Surreal.Audio.Clips;

namespace Surreal.Audio;

/// <summary>A no-op <see cref="IAudioServer"/> for headless environments and testing.</summary>
public sealed class HeadlessAudioServer : IAudioServer
{
  private int nextClipId = 0;
  private int nextSourceId = 0;

  public AudioHandle CreateAudioClip()
  {
    return new AudioHandle(Interlocked.Increment(ref nextClipId));
  }

  public void DeleteAudioClip(AudioHandle clip)
  {
    // no-op
  }

  public void WriteAudioClipData<T>(AudioHandle clip, AudioSampleRate sampleRate, ReadOnlySpan<T> data) where T : unmanaged
  {
    // no-op
  }

  public AudioHandle CreateAudioSource()
  {
    return new AudioHandle(Interlocked.Increment(ref nextSourceId));
  }

  public bool IsAudioSourcePlaying(AudioHandle handle)
  {
    return false;
  }

  public void PlayAudioSource(AudioHandle source, AudioHandle clip)
  {
    // no-op
  }

  public void StopAudioSource(AudioHandle source)
  {
    // no-op
  }

  public void SetAudioSourceVolume(AudioHandle source, float value)
  {
    // no-op
  }

  public void SetAudioSourceLooping(AudioHandle source, bool value)
  {
    // no-op
  }

  public void DeleteAudioSource(AudioHandle source)
  {
    // no-op
  }
}
