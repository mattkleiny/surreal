using NAudio.Wave;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>A buffer of waveform data for use in audio playback.</summary>
public sealed class AudioBuffer : AudioResource, IHasSizeEstimate
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

  public Memory<byte> Memory => buffer.Memory;
  public Span<byte>   Span   => Memory.Span;
  public Size         Size   => buffer.Memory.Span.CalculateSize();

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      buffer.Dispose();
    }

    base.Dispose(managed);
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="AudioBuffer"/>s.</summary>
public sealed class AudioBufferLoader : AssetLoader<AudioBuffer>
{
  public override async Task<AudioBuffer> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();

    await using WaveStream reader = context.Path.Extension switch
    {
      ".wav"  => new WaveFileReader(stream),
      ".mp3"  => new Mp3FileReader(stream),
      ".aiff" => new AiffFileReader(stream),

      _ => throw new UnsupportedAudioFormatException($"An unrecognized audio file format was requested: {context.Path}")
    };

    var format = reader.WaveFormat;

    var sampleRate = new AudioSampleRate(format.SampleRate, format.Channels, format.BitsPerSample);
    var buffer = new AudioBuffer(reader.TotalTime, sampleRate);

    while (reader.CanRead)
    {
      var bytesRead = await reader.ReadAsync(buffer.Memory, cancellationToken);
      if (bytesRead <= 0)
      {
        break;
      }
    }

    return buffer;
  }
}

/// <summary>Indicates an attempt to us an unsupported audio format.</summary>
public sealed class UnsupportedAudioFormatException : Exception
{
  public UnsupportedAudioFormatException(string message)
    : base(message)
  {
  }
}
