using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>
/// Contains information on audio sampling rate for use in audio calculations.
/// </summary>
public readonly record struct AudioSampleRate(int Frequency, int Channels, int BitsPerSample)
{
  /// <summary>
  /// A standard audio sample rate of 44.1khz, 2 channels, and 16 bits per sample.
  /// </summary>
  public static AudioSampleRate Standard => new(44_100, 2, 16);

  /// <summary>
  /// The number of bit per second that this sample rate represents.
  /// </summary>
  public int BitsPerSecond => Frequency * Channels * BitsPerSample;

  /// <summary>
  /// The number of bytes per second that this sample rate represents.
  /// </summary>
  public float BytesPerSecond => BitsPerSecond / 8f;

  /// <summary>
  /// Calculates the size of a buffer for the given duration at this rate.
  /// </summary>
  public Size CalculateSize(TimeSpan duration)
  {
    return (int)Math.Ceiling(duration.TotalSeconds * BytesPerSecond);
  }

  public override string ToString()
  {
    return $"{Frequency:N0} hz * {Channels} channels * {BitsPerSample} bits per sample = {BitsPerSecond:N0}bps";
  }
}
