﻿using Surreal.Audio.Clips;
using Surreal.IO;

namespace Surreal.Assets.Importers;

internal sealed class AudioClipImporter : AssetImporter<AudioClip>
{
  protected override bool CanHandlePath(string path)
  {
    return path.EndsWith(".wav");
  }

  public override Task<AudioClip> ImportAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
