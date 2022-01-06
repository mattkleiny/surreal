using Surreal.Assets;
using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>Represents source data that can be uploaded to an <see cref="AudioClip"/>.</summary>
public interface IAudioData
{
  TimeSpan        Duration { get; }
  AudioSampleRate Rate     { get; }
  Size            Size     { get; }
  Span<byte>      Buffer   { get; }
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

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="AudioClip"/>s.</summary>
[RegisterService(typeof(IAssetLoader))]
public sealed class AudioClipLoader : AssetLoader<AudioClip>
{
  private readonly IAudioDevice device;

  public AudioClipLoader(IAudioDevice device)
  {
    this.device = device;
  }

  public override async Task<AudioClip> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var buffer = await context.Manager.LoadAsset<AudioBuffer>(context.Path, cancellationToken);

    return device.CreateAudioClip(buffer);
  }
}
