using Surreal.Resources;

namespace Surreal.Audio.Clips;

/// <summary>
/// The <see cref="ResourceLoader{T}" /> for <see cref="AudioClip" />s.
/// </summary>
public sealed class AudioClipLoader : ResourceLoader<AudioClip>
{
  private readonly IAudioBackend _backend;

  public AudioClipLoader(IAudioBackend backend)
  {
    _backend = backend;
  }

  public override async Task<AudioClip> LoadAsync(ResourceContext context, CancellationToken cancellationToken)
  {
    var buffer = await context.LoadAsync<AudioBuffer>(context.Path, cancellationToken);
    var clip = new AudioClip(_backend);

    clip.Write<byte>(buffer.Duration, buffer.Rate, buffer.Span);

    return clip;
  }
}
