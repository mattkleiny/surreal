using Surreal.Assets;
using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// An <see cref="IAssetLoader"/> for <see cref="AsepriteFile"/>s.
/// </summary>
[RegisterService(typeof(IAssetLoader))]
public sealed class AsepriteFileLoader : AssetLoader<AsepriteFile>
{
  public override async Task<AsepriteFile> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();

    return AsepriteFile.Load(stream);
  }
}
