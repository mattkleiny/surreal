using Surreal.Assets;

namespace Surreal.Audio.Clips;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="AudioClip" />s.
/// </summary>
public sealed class AudioClipLoader(IAudioBackend backend) : AssetLoader<AudioClip>
{
  public override async Task<AudioClip> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    var buffer = await context.LoadAsync<AudioBuffer>(context.Path, cancellationToken);
    var clip = new AudioClip(backend);

    clip.Write<byte>(buffer.Duration, buffer.Rate, buffer.Span);

    return clip;
  }
}
