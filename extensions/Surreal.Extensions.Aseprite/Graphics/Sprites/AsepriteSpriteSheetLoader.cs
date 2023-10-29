using Surreal.Assets;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// The <see cref="IAssetLoader"/> for <see cref="AsepriteFile"/>-based <see cref="SpriteSheet"/>s.
/// </summary>
public sealed class AsepriteSpriteSheetLoader : AssetLoader<SpriteSheet>
{
  public override bool CanHandle(AssetContext context)
  {
    return base.CanHandle(context) &&
           context.Path.Extension.EndsWith("ase") ||
           context.Path.Extension.EndsWith("aseprite");
  }

  public override async Task<SpriteSheet> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    var asepriteFile = await context.LoadAsync<AsepriteFile>(context.Path, cancellationToken);

    // TODO: convert this to a sprite sheet

    throw new NotImplementedException();
  }
}
