using System;
using Surreal.Data;

namespace Surreal.Audio.Clips {
  public readonly struct AudioSampleRate {
    public static AudioSampleRate None     => default;
    public static AudioSampleRate Standard => new(frequency: 44_100, channels: 2, bitsPerSample: 16);

    public readonly int Frequency;
    public readonly int Channels;
    public readonly int BitsPerSample;

    public int   BitsPerSecond  => Frequency * Channels * BitsPerSample;
    public float BytesPerSecond => BitsPerSecond / 8f;

    public AudioSampleRate(int frequency, int channels, int bitsPerSample) {
      Frequency     = frequency;
      Channels      = channels;
      BitsPerSample = bitsPerSample;
    }

    public void Deconstruct(out int rate, out int channels, out int bitsPerSample) {
      rate          = Frequency;
      channels      = Channels;
      bitsPerSample = BitsPerSample;
    }

    public Size CalculateSize(TimeSpan duration) {
      return new((int) Math.Ceiling(duration.TotalSeconds * BytesPerSecond));
    }

    public override string ToString() {
      return $"{Frequency.ToString("N0")} hz * {Channels.ToString()} channels * {BitsPerSample.ToString()} bits per sample = {BitsPerSecond.ToString()}bps";
    }

    public bool Equals(AudioSampleRate other) {
      return Frequency == other.Frequency &&
             Channels == other.Channels &&
             BitsPerSample == other.BitsPerSample;
    }

    public override bool Equals(object? obj) => obj is AudioSampleRate other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Frequency, Channels, BitsPerSample);
  }
}