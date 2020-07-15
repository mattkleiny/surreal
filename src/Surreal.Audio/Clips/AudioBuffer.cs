using System;
using System.Threading.Tasks;
using NAudio.Wave;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Audio.Clips {
  public sealed class AudioBuffer : IAudioData, IDisposable, IHasSizeEstimate {
    private readonly IDisposableBuffer<byte> buffer;

    public AudioBuffer(TimeSpan duration, AudioSampleRate rate) {
      Duration = duration;
      Rate     = rate;

      buffer = Buffers.AllocateOffHeap<byte>(rate.CalculateSize(duration));
    }

    public TimeSpan        Duration { get; }
    public AudioSampleRate Rate     { get; }

    public Size       Size => buffer.Size;
    public Span<byte> Span => buffer.Span;

    public void Dispose() => buffer.Dispose();

    public sealed class Loader : AssetLoader<AudioBuffer> {
      public override async Task<AudioBuffer> LoadAsync(Path path, IAssetLoaderContext context) {
        await using var stream = await path.OpenInputStreamAsync();
        await using WaveStream reader = path.GetExtension() switch {
            ".wav"  => new WaveFileReader(stream),
            ".mp3"  => new Mp3FileReader(stream),
            ".aiff" => new AiffFileReader(stream),
            _       => throw new Exception($"An unrecognized file format was requested: {path}")
        };

        var format = reader.WaveFormat;

        var rate   = new AudioSampleRate(format.SampleRate, format.Channels, format.BitsPerSample);
        var buffer = new AudioBuffer(reader.TotalTime, rate);

        reader.Read(buffer.Span);

        return buffer;
      }
    }
  }
}