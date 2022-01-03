using NAudio.Wave;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Audio.Clips;

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
  public Span<byte>      Buffer   => buffer.Span;
  public Size            Size     => buffer.Span.CalculateSize();

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
  public override async Task<AudioBuffer> LoadAsync(VirtualPath path, IAssetContext context, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    await using WaveStream reader = path.Extension switch
    {
      ".wav"  => new WaveFileReader(stream),
      ".mp3"  => new Mp3FileReader(stream),
      ".aiff" => new AiffFileReader(stream),

      _ => throw new UnsupportedAudioFormatException($"An unrecognized audio file format was requested: {path}"),
    };

    var format = reader.WaveFormat;

    var rate   = new AudioSampleRate(format.SampleRate, format.Channels, format.BitsPerSample);
    var buffer = new AudioBuffer(reader.TotalTime, rate);

    reader.Read(buffer.Buffer);

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
