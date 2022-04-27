using System.Runtime.CompilerServices;
using OpenTK.Audio.OpenAL;
using Surreal.Audio;
using Surreal.Audio.Clips;

namespace Surreal.Internal.Audio;

internal sealed class OpenTKAudioServer : IAudioServer, IDisposable
{
  private readonly ALDevice device;
  private readonly ALContext context;

  private bool isDisposed;

  public OpenTKAudioServer()
  {
    device  = ALC.OpenDevice(null);
    context = ALC.CreateContext(device, ref Unsafe.NullRef<int>());

    ALC.MakeContextCurrent(context);
  }

  public AudioHandle CreateAudioClip()
  {
    return new AudioHandle(AL.GenBuffer());
  }

  public void DeleteAudioClip(AudioHandle clip)
  {
    AL.DeleteBuffer(clip);
  }

  public unsafe void WriteAudioClipData<T>(AudioHandle clip, AudioSampleRate sampleRate, ReadOnlySpan<T> data)
    where T : unmanaged
  {
    var (frequency, channels, bitsPerSample) = sampleRate;

    var format = GetSoundFormat(channels, bitsPerSample);
    var bytes = data.Length * sizeof(T);

    fixed (T* pointer = data)
    {
      AL.BufferData(clip, format, pointer, bytes, frequency);
    }
  }

  public AudioHandle CreateAudioSource()
  {
    return new AudioHandle(AL.GenSource());
  }

  public void PlayAudioSource(AudioHandle source, AudioHandle clip)
  {
    AL.Source(source, ALSourcei.Buffer, clip);
    AL.SourcePlay(source);
  }

  public void StopAudioSource(AudioHandle source)
  {
    AL.SourceStop(source);
  }

  public void SetAudioSourceVolume(AudioHandle source, float value)
  {
    AL.Source(source, ALSourcef.Gain, value);
  }

  public void SetAudioSourceLooping(AudioHandle source, bool value)
  {
    AL.Source(source, ALSourceb.Looping, value);
  }

  public void DeleteAudioSource(AudioHandle source)
  {
    AL.DeleteSource(source);
  }

  public void Dispose()
  {
    if (!isDisposed)
    {
      ALC.MakeContextCurrent(ALContext.Null);
      ALC.DestroyContext(context);

      ALC.CloseDevice(device);

      isDisposed = true;
    }
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
