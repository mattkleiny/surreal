using Surreal.Assets;

namespace Surreal.Audio.Clips;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="AudioClip" />s.
/// </summary>
public sealed class AudioClipLoader(IAudioDevice device) : AssetLoader<AudioClip>
{
  public override async Task<AudioClip> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    var buffer = await context.LoadAsync<AudioBuffer>(context.Path, cancellationToken);
    var clip = new AudioClip(device);

    clip.Write<byte>(buffer.Value.Duration, buffer.Value.Rate, buffer.Value.Span);

    return clip;
  }
}
