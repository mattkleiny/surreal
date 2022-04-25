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

  public void DeleteAudioClip(AudioHandle handle)
  {
    // no-op
  }

  public void WriteAudioClipData<T>(AudioHandle handle, AudioSampleRate sampleRate, ReadOnlySpan<T> buffer) where T : unmanaged
  {
    // no-op
  }

  public AudioHandle CreateAudioSource()
  {
    return new AudioHandle(Interlocked.Increment(ref nextSourceId));
  }

  public void DeleteAudioSource(AudioHandle handle)
  {
    // no-op
  }
}