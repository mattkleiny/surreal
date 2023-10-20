﻿using Surreal.Resources;

namespace Surreal.Audio.Clips;

/// <summary>
/// The <see cref="ResourceLoader{T}" /> for <see cref="AudioClip" />s.
/// </summary>
public sealed class AudioClipLoader(IAudioBackend backend) : ResourceLoader<AudioClip>
{
  public override async Task<AudioClip> LoadAsync(ResourceContext context, CancellationToken cancellationToken)
  {
    var buffer = await context.LoadAsync<AudioBuffer>(context.Path, cancellationToken);
    var clip = new AudioClip(backend);

    clip.Write<byte>(buffer.Duration, buffer.Rate, buffer.Span);

    return clip;
  }
}
