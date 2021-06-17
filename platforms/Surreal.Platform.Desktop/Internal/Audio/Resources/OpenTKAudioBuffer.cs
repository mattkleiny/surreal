using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NAudio.Wave;
using Surreal.Assets;
using Surreal.Audio.Clips;
using Surreal.Data;
using Surreal.Data.VFS;

namespace Surreal.Platform.Internal.Audio.Resources {
  [DebuggerDisplay("Audio Clip {Duration} ~{Size}")]
  internal sealed class OpenTKAudioBuffer : AudioBuffer {
    private readonly IDisposableBuffer<byte> buffer;

    public OpenTKAudioBuffer(TimeSpan duration, AudioSampleRate rate) {
      Duration = duration;
      Rate     = rate;

      buffer = Buffers.AllocateNative<byte>(rate.CalculateSize(duration));
    }

    public override TimeSpan        Duration { get; }
    public override AudioSampleRate Rate     { get; }

    public override Size       Size => buffer.Size;
    public override Span<byte> Data => buffer.Data;

    public sealed class Loader : AssetLoader<OpenTKAudioBuffer> {
      public override async Task<OpenTKAudioBuffer> LoadAsync(Path path, IAssetLoaderContext context) {
        await using var stream = await path.OpenInputStreamAsync();
        await using WaveStream reader = path.GetExtension() switch {
          ".wav"  => new WaveFileReader(stream),
          ".mp3"  => new Mp3FileReader(stream),
          ".aiff" => new AiffFileReader(stream),
          _       => throw new Exception($"An unrecognized file format was requested: {path}"),
        };

        var format = reader.WaveFormat;

        var rate   = new AudioSampleRate(format.SampleRate, format.Channels, format.BitsPerSample);
        var buffer = new OpenTKAudioBuffer(reader.TotalTime, rate);

        reader.Read(buffer.Data);

        return buffer;
      }
    }
  }
}