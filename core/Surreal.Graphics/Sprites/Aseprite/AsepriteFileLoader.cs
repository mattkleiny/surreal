using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Sprites.Aseprite;

/// <summary>
/// An asset loader for <see cref="AsepriteFile"/> assets.
/// </summary>
public sealed class AsepriteFileLoader : AssetLoader<AsepriteFile>
{
  private static ImmutableHashSet<string> Extensions { get; } = [".ase", ".aseprite"];

  public override bool CanHandle(AssetId id)
  {
    return base.CanHandle(id) && Extensions.Contains(id.Path.Extension);
  }

  public override async Task<AsepriteFile> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    await using var stream = context.Path.OpenInputStream();

    return AsepriteFile.Load(stream);
  }
}
