using Silk.NET.OpenAL;
using Surreal.Audio.Clips;

namespace Surreal.Audio;

internal sealed class SilkAudioBackend(AL al) : IAudioBackend
{
  public AudioHandle CreateAudioClip()
  {
    return new AudioHandle(al.GenBuffer());
  }

  public unsafe void WriteAudioClipData<T>(AudioHandle clip, AudioSampleRate sampleRate, ReadOnlySpan<T> data)
    where T : unmanaged
  {
    fixed (T* pointer = data)
    {
      al.BufferData(
        buffer: clip,
        format: BufferFormat.Stereo16,
        data: pointer,
        size: data.Length * sizeof(T),
        frequency: sampleRate.Frequency
      );
    }
  }

  public void DeleteAudioClip(AudioHandle clip)
  {
    al.DeleteBuffer(clip);
  }

  public AudioHandle CreateAudioSource()
  {
    return new AudioHandle(al.GenSource());
  }

  public bool IsAudioSourcePlaying(AudioHandle handle)
  {
    al.GetSourceProperty(handle, GetSourceInteger.SourceState, out int value);

    return value == (int)SourceState.Playing;
  }

  public void PlayAudioSource(AudioHandle source, AudioHandle clip)
  {
    al.SetSourceProperty(source, SourceInteger.Buffer, clip);
    al.SourcePlay(source);
  }

  public void StopAudioSource(AudioHandle source)
  {
    al.SourceStop(source);
  }

  public void SetAudioSourcePosition(AudioHandle source, Vector3 value)
  {
    al.SetSourceProperty(source, SourceVector3.Position, value);
  }

  public void SetAudioSourceVolume(AudioHandle source, float value)
  {
    al.SetSourceProperty(source, SourceFloat.Gain, value);
  }

  public void SetAudioSourceLooping(AudioHandle source, bool value)
  {
    al.SetSourceProperty(source, SourceBoolean.Looping, value);
  }

  public void DeleteAudioSource(AudioHandle source)
  {
    al.DeleteSource(source);
  }
}
