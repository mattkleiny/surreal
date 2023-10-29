using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// An <see cref="IAssetLoader"/> for <see cref="AsepriteFile"/>s.
/// </summary>
public sealed class AsepriteFileLoader : AssetLoader<AsepriteFile>
{
  public override async Task<AsepriteFile> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    await using var stream = context.Path.OpenInputStream();

    return AsepriteFile.Load(stream);
  }
}
