using System;
using System.Threading.Tasks;
using NAudio.Wave;
using Surreal.Assets;
using Surreal.IO;
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
    public Size            Size     => buffer.Size;
    public Span<byte>      Data     => buffer.Data;

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
    public override async Task<AudioBuffer> LoadAsync(Path path, IAssetResolver context)
    {
      await using var stream = await path.OpenInputStreamAsync();
      await using WaveStream reader = path.Extension switch
      {
        ".wav"  => new WaveFileReader(stream),
        ".mp3"  => new Mp3FileReader(stream),
        ".aiff" => new AiffFileReader(stream),
        _       => throw new Exception($"An unrecognized file format was requested: {path}"),
      };

      var format = reader.WaveFormat;

      var rate   = new AudioSampleRate(format.SampleRate, format.Channels, format.BitsPerSample);
      var buffer = new AudioBuffer(reader.TotalTime, rate);

      reader.Read(buffer.Data);

      return buffer;
    }
  }
}
