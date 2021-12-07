using NAudio.Wave;
using Surreal.Assets;
using Surreal.IO;
using Path = Surreal.IO.Path;

namespace Surreal.Audio.Clips;

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="AudioBuffer"/>s.</summary>
public sealed class AudioBufferLoader : AssetLoader<AudioBuffer>
{
  public override async Task<AudioBuffer> LoadAsync(Path path, IAssetResolver resolver)
  {
    await using var stream = await path.OpenInputStreamAsync();

    await using WaveStream reader = path.Extension switch
    {
      ".wav"  => new WaveFileReader(stream),
      ".mp3"  => new Mp3FileReader(stream),
      ".aiff" => new AiffFileReader(stream),

      _ => throw new Exception($"An unrecognized file format was requested: {path}")
    };

    var format = reader.WaveFormat;

    var rate   = new AudioSampleRate(format.SampleRate, format.Channels, format.BitsPerSample);
    var buffer = new AudioBuffer(reader.TotalTime, rate);

    reader.Read(buffer.Data);

    return buffer;
  }
}
