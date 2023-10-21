using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>
/// A buffer of waveform data for use in audio playback.
/// </summary>
public sealed class AudioBuffer(TimeSpan duration, AudioSampleRate rate) : AudioAsset, IHasSizeEstimate
{
  private readonly IDisposableBuffer<byte> _buffer = Buffers.AllocateNative<byte>(rate.CalculateSize(duration));

  public TimeSpan Duration { get; } = duration;
  public AudioSampleRate Rate { get; } = rate;

  public Memory<byte> Memory => _buffer.Memory;
  public Span<byte> Span => _buffer.Span;
  public Size Size => Span.CalculateSize();

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      _buffer.Dispose();
    }

    base.Dispose(managed);
  }
}

/// <summary>
/// Indicates an attempt to us an unsupported audio format.
/// </summary>
public sealed class UnsupportedAudioFormatException(string message) : ApplicationException(message);
