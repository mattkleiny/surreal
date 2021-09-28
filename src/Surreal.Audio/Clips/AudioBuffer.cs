using System;
using Surreal.Content;
using Surreal.Memory;

namespace Surreal.Audio.Clips
{
  /// <summary>A buffer of audio data for use in audio playback.</summary>
  public sealed class AudioBuffer : AudioResource, IAudioData, IHasSizeEstimate
  {
    private readonly IDisposableBuffer<byte> buffer;

    public AudioBuffer(TimeSpan duration, AudioSampleRate rate)
    {
      Duration = duration;
      Rate     = rate;

      buffer = Buffers.AllocateNative<byte>(rate.CalculateSize(duration));
    }

    public TimeSpan        Duration { get; }
    public AudioSampleRate Rate     { get; }
    public Span<byte>      Data     => buffer.Data;
    public Size            Size     => buffer.Data.CalculateSize();

    protected override void Dispose(bool managed)
    {
      if (managed)
      {
        buffer.Dispose();
      }

      base.Dispose(managed);
    }
  }
}
