using System;
using System.Threading.Tasks;
using NAudio.Wave;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Audio.Clips {
  public sealed class WaveData : IAudioData, IDisposable, IHasSizeEstimate {
    private readonly IDisposableBuffer<byte> buffer;

    public WaveData(int sampleRate, int channels, int bitsPerSample) {
      SampleRate    = sampleRate;
      Channels      = channels;
      BitsPerSample = bitsPerSample;

      buffer = Buffers.AllocateOffHeap<byte>(SampleRate * Channels * BitsPerSample);
    }

    public TimeSpan Duration => TimeSpan.FromSeconds(Span.Length / (SampleRate * Channels * BitsPerSample / 8f));

    public int SampleRate    { get; }
    public int Channels      { get; }
    public int BitsPerSample { get; }

    public Size       Size => buffer.Size;
    public Span<byte> Span => buffer.Span;

    public void Dispose() => buffer.Dispose();

    public sealed class Loader : AssetLoader<WaveData> {
      public override async Task<WaveData> LoadAsync(Path path, IAssetLoaderContext context) {
        await using var stream = await path.OpenInputStreamAsync();
        await using WaveStream reader = System.IO.Path.GetExtension(path.Target) switch {
            ".wav"  => new WaveFileReader(stream),
            ".mp3"  => new Mp3FileReader(stream),
            ".aiff" => new AiffFileReader(stream),
            _       => throw new Exception($"An unrecognized file format was requested: {path}")
        };

        var format = reader.WaveFormat;
        var result = new WaveData(format.SampleRate, format.Channels, format.BitsPerSample);

        reader.Read(result.Span);

        return result;
      }
    }
  }
}