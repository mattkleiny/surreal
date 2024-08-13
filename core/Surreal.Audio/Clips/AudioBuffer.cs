using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>
/// A buffer of waveform data for use in audio playback.
/// </summary>
public sealed class AudioBuffer(TimeSpan duration, AudioSampleRate rate) : Disposable
{
  private readonly IDisposableBuffer<byte> _buffer = Buffers.AllocateNative<byte>(rate.CalculateSize(duration));

  /// <summary>
  /// The duration of the buffer's audio
  /// </summary>
  public TimeSpan Duration { get; } = duration;

  /// <summary>
  /// The sample rate of the buffer's audio
  /// </summary>
  public AudioSampleRate Rate { get; } = rate;

  /// <summary>
  /// The buffer's data as a memory of bytes.
  /// </summary>
  public Memory<byte> Memory => _buffer.Memory;

  /// <summary>
  /// The buffer's data as a span of bytes.
  /// </summary>
  public Span<byte> Span => _buffer.Span;

  /// <summary>
  /// The size of the buffer in bytes.
  /// </summary>
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
