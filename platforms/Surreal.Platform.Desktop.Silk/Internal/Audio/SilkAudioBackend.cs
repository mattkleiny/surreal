using Surreal.Audio;
using Surreal.Audio.Clips;

namespace Surreal.Internal.Audio;

internal sealed class SilkAudioBackend : IAudioBackend
{
  public AudioHandle CreateAudioClip()
  {
    throw new NotImplementedException();
  }

  public void WriteAudioClipData<T>(AudioHandle clip, AudioSampleRate sampleRate, ReadOnlySpan<T> data) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void DeleteAudioClip(AudioHandle clip)
  {
    throw new NotImplementedException();
  }

  public AudioHandle CreateAudioSource()
  {
    throw new NotImplementedException();
  }

  public bool IsAudioSourcePlaying(AudioHandle handle)
  {
    throw new NotImplementedException();
  }

  public void PlayAudioSource(AudioHandle source, AudioHandle clip)
  {
    throw new NotImplementedException();
  }

  public void StopAudioSource(AudioHandle source)
  {
    throw new NotImplementedException();
  }

  public void SetAudioSourceVolume(AudioHandle source, float value)
  {
    throw new NotImplementedException();
  }

  public void SetAudioSourceLooping(AudioHandle source, bool value)
  {
    throw new NotImplementedException();
  }

  public void DeleteAudioSource(AudioHandle source)
  {
    throw new NotImplementedException();
  }
}
