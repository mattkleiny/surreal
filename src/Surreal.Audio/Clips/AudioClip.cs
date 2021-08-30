using System;
using System.Threading.Tasks;
using Surreal.Content;
using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Audio.Clips
{
  /// <summary>Represents source data that can be uploaded to an <see cref="AudioClip"/>.</summary>
  public interface IAudioData
  {
    TimeSpan        Duration { get; }
    AudioSampleRate Rate     { get; }
    Size            Size     { get; }
    Span<byte>      Data     { get; }
  }

  /// <summary>A clip of audio that can be played back via an audio device.</summary>
  public abstract class AudioClip : AudioResource, IHasSizeEstimate
  {
    private IAudioData? data;

    public TimeSpan Duration => data?.Duration ?? TimeSpan.MinValue;
    public Size     Size     => data?.Size ?? Size.Zero;

    public void Upload(IAudioData newData)
    {
      Upload(data, newData);
      data = newData;
    }

    protected abstract void Upload(IAudioData? existingData, IAudioData newData);
  }

  public sealed class AudioClipLoader : AssetLoader<AudioClip>
  {
    private readonly IAudioDevice device;

    public AudioClipLoader(IAudioDevice device)
    {
      this.device = device;
    }

    public override async Task<AudioClip> LoadAsync(Path path, IAssetResolver resolver)
    {
      var buffer = await resolver.LoadAsset<AudioBuffer>(path);

      return device.CreateAudioClip(buffer.Data);
    }
  }
}
