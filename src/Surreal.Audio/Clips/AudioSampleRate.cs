using System;
using Surreal.Memory;

namespace Surreal.Audio.Clips
{
  /// <summary>Contains information on audio sampling rate for use in audio calculations.</summary>
  public readonly record struct AudioSampleRate(int Frequency, int Channels, int BitsPerSample)
  {
    public static AudioSampleRate None     => default;
    public static AudioSampleRate Standard => new(Frequency: 44_100, Channels: 2, BitsPerSample: 16);

    public int   BitsPerSecond  => Frequency * Channels * BitsPerSample;
    public float BytesPerSecond => BitsPerSecond / 8f;

    public Size CalculateSize(TimeSpan duration)
    {
      return new((int)Math.Ceiling(duration.TotalSeconds * BytesPerSecond));
    }

    public override string ToString()
    {
      return $"{Frequency.ToString("N0")} hz * {Channels.ToString()} channels * {BitsPerSample.ToString()} bits per sample = {BitsPerSecond.ToString()}bps";
    }
  }
}
