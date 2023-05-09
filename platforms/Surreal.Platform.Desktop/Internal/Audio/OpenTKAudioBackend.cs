using Surreal.Audio;
using Surreal.Audio.Clips;

namespace Surreal.Internal.Audio;

internal sealed class OpenTKAudioBackend : IAudioBackend, IDisposable
{
  private nint _nextAudioClipId;
  private nint _nextAudioSourceId;

  public AudioHandle CreateAudioClip()
  {
    return new AudioHandle(_nextAudioClipId++);
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
    return new AudioHandle(_nextAudioSourceId++);
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

  public void SetAudioSourceVolume(AudioHandle source, float value)
  {
  }

  public void SetAudioSourceLooping(AudioHandle source, bool value)
  {
  }

  public void DeleteAudioSource(AudioHandle source)
  {
  }

  public void Dispose()
  {
  }
}


