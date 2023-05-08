using Surreal.Assets;

namespace Surreal.Audio.Clips;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="AudioClip" />s.
/// </summary>
public sealed class AudioClipLoader : AssetLoader<AudioClip>
{
  private readonly IAudioServer _server;

  public AudioClipLoader(IAudioServer server)
  {
    _server = server;
  }

  public override async Task<AudioClip> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var buffer = await context.LoadAsync<AudioBuffer>(context.Path, cancellationToken);
    var clip = new AudioClip(_server);

    clip.Write<byte>(buffer.Duration, buffer.Rate, buffer.Span);

    return clip;
  }
}
