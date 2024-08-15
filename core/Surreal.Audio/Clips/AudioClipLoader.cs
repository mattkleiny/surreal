using NAudio.Wave;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>
/// Indicates an attempt to us an unsupported audio format.
/// </summary>
public sealed class UnsupportedAudioFormatException(string message) : ApplicationException(message);

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="AudioClip" />s.
/// </summary>
public sealed class AudioClipLoader(IAudioDevice device) : AssetLoader<AudioClip>
{
  public override async Task<AudioClip> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    await using var stream =  context.Path.OpenInputStream();

    // open the audio file and read the audio data
    await using WaveStream reader = context.Path.Extension switch
    {
      ".wav" => new WaveFileReader(stream),
      ".mp3" => new Mp3FileReader(stream),
      ".aiff" => new AiffFileReader(stream),

      _ => throw new UnsupportedAudioFormatException($"An unrecognized audio file format was requested: {context.Path}")
    };

    var format = reader.WaveFormat;
    var sampleRate = new AudioSampleRate(format.SampleRate, format.Channels, format.BitsPerSample);

    // allocate a buffer for the audio data
    var bufferSize = sampleRate.CalculateSize(reader.TotalTime);
    using var buffer = Buffers.AllocateNative<byte>(bufferSize);

    while (reader.CanRead)
    {
      var bytesRead = await reader.ReadAsync(buffer.Memory, cancellationToken);
      if (bytesRead <= 0)
      {
        break;
      }
    }

    // create the audio clip and write the audio data to it
    var clip = new AudioClip(device);

    clip.Write<byte>(reader.TotalTime, sampleRate, buffer.Span);

    return clip;
  }
}
