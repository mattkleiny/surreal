using Surreal.Assets;

namespace Surreal.Graphics.Sprites.Aseprite;

/// <summary>
/// The <see cref="IAssetLoader"/> for <see cref="AsepriteFile"/>-based <see cref="SpriteSheet"/>s.
/// </summary>
public sealed class AsepriteSpriteSheetLoader : AssetLoader<SpriteSheet>
{
  public override bool CanHandle(AssetId id)
  {
    return base.CanHandle(id) &&
           id.Path.Extension.EndsWith("ase") ||
           id.Path.Extension.EndsWith("aseprite");
  }

  public override async Task<SpriteSheet> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    var asepriteFile = await context.LoadDependencyAsync<AsepriteFile>(context.Path, cancellationToken);

    // TODO: convert this to a sprite sheet

    throw new NotImplementedException();
  }
}
