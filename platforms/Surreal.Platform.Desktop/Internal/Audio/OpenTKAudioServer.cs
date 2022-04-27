using OpenTK.Audio.OpenAL;
using Surreal.Audio;
using Surreal.Audio.Clips;

namespace Surreal.Internal.Audio;

internal sealed class OpenTKAudioServer : IAudioServer
{
  public AudioHandle CreateAudioClip()
  {
    return new AudioHandle(AL.GenBuffer());
  }

  public void DeleteAudioClip(AudioHandle handle)
  {
    AL.DeleteBuffer(handle);
  }

  public unsafe void WriteAudioClipData<T>(AudioHandle handle, AudioSampleRate sampleRate, ReadOnlySpan<T> data)
    where T : unmanaged
  {
    var (frequency, channels, bitsPerSample) = sampleRate;

    var format = GetSoundFormat(channels, bitsPerSample);
    var bytes = data.Length * sizeof(T);

    fixed (T* pointer = data)
    {
      AL.BufferData(handle, format, pointer, bytes, frequency);
    }
  }

  public AudioHandle CreateAudioSource()
  {
    return new AudioHandle(AL.GenSource());
  }

  public void DeleteAudioSource(AudioHandle handle)
  {
    AL.DeleteSource(handle);
  }

  private static ALFormat GetSoundFormat(int channels, int bits)
  {
    return channels switch
    {
      1 => bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16,
      2 => bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16,

      _ => throw new NotSupportedException("The specified sound format is not supported."),
    };
  }
}
