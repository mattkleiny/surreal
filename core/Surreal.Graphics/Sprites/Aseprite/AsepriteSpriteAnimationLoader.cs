using Surreal.Assets;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Sprites.Aseprite;

/// <summary>
/// An asset loader for <see cref="AsepriteFile"/> assets.
/// </summary>
public sealed class AsepriteSpriteAnimationLoader : AssetLoader<SpriteAnimation>
{
  private static ImmutableHashSet<string> Extensions { get; } = [".ase", ".aseprite"];

  public override bool CanHandle(AssetId id)
  {
    return base.CanHandle(id) && Extensions.Contains(id.Path.Extension);
  }

  public override async Task<SpriteAnimation> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    var metadata = await context.LoadAsync<AsepriteFile>(context.Path, cancellationToken);
    var texture = await context.LoadAsync<TextureAtlas>(context.Path, cancellationToken);

    throw new NotImplementedException();
  }
}
