using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>Contains information on audio sampling rate for use in audio calculations.</summary>
public readonly record struct AudioSampleRate(int Frequency, int Channels, int BitsPerSample)
{
  public static AudioSampleRate Standard => new(44_100, 2, 16);

  public int BitsPerSecond => Frequency * Channels * BitsPerSample;
  public float BytesPerSecond => BitsPerSecond / 8f;

  public Size CalculateSize(TimeSpan duration)
  {
    return (int) Math.Ceiling(duration.TotalSeconds * BytesPerSecond);
  }

  public override string ToString()
  {
    return $"{Frequency:N0} hz * {Channels} channels * {BitsPerSample} bits per sample = {BitsPerSecond}bps";
  }
}

