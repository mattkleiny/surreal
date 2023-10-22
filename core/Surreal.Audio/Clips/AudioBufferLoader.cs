using NAudio.Wave;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Audio.Clips;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="AudioBuffer" />s.
/// </summary>
[RegisterService(typeof(IAssetLoader))]
public sealed class AudioBufferLoader : AssetLoader<AudioBuffer>
{
  public override async Task<AudioBuffer> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();

    await using WaveStream reader = context.Path.Extension switch
    {
      ".wav" => new WaveFileReader(stream),
      ".mp3" => new Mp3FileReader(stream),
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
