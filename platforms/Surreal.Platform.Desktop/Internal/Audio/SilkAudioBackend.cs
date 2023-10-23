using Silk.NET.OpenAL;
using Surreal.Audio.Clips;

namespace Surreal.Audio;

internal sealed unsafe class SilkAudioBackend : IAudioBackend, IDisposable
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  private static ALContext alc = ALContext.GetApi(soft: true);

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  private static readonly AL al = AL.GetApi(soft: true);

  private readonly Context* _context;

  public SilkAudioBackend()
  {
    var device = alc.OpenDevice(string.Empty); // the default device

    _context = alc.CreateContext(device, null);
    if (_context == null)
    {
      throw new InvalidOperationException("Failed to create OpenAL context.");
    }

    alc.MakeContextCurrent(_context);
  }

  public void SetAudioListenerGain(float gain)
  {
    al.SetListenerProperty(ListenerFloat.Gain, gain);
  }

  public void SetAudioListenerPosition(Vector3 vector3)
  {
    al.SetListenerProperty(ListenerVector3.Position, vector3.X, vector3.Y, vector3.Z);
  }

  public void SetAudioListenerVelocity(Vector3 vector3)
  {
    al.SetListenerProperty(ListenerVector3.Velocity, vector3.X, vector3.Y, vector3.Z);
  }

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
        format: ConvertSampleRate(sampleRate),
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
    var source = al.GenSource();

    al.SetSourceProperty(source, SourceFloat.Gain, 1f);
    al.SetSourceProperty(source, SourceVector3.Position, Vector3.Zero);
    al.SetSourceProperty(source, SourceBoolean.Looping, false);

    return new AudioHandle(source);
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

  public void SetAudioSourceGain(AudioHandle source, float value)
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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static BufferFormat ConvertSampleRate(AudioSampleRate sampleRate)
  {
    return sampleRate switch
    {
      { BitsPerSample: 16, Channels: > 1 } => BufferFormat.Stereo16,
      { BitsPerSample: 16, Channels: 1 } => BufferFormat.Mono16,
      { BitsPerSample: 8, Channels: > 1 } => BufferFormat.Stereo8,
      { BitsPerSample: 8, Channels: 1 } => BufferFormat.Mono8,

      _ => throw new NotSupportedException($"Unsupported sample rate: {sampleRate}")
    };
  }

  public void Dispose()
  {
    if (_context != null)
    {
      alc.DestroyContext(_context);
    }
  }
}
