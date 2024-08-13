using Surreal.Assets;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Sprites.Aseprite;

/// <summary>
/// An asset loader for <see cref="AsepriteFile"/>-based texture atlas.
/// </summary>
public sealed class AsepriteTextureLoader : AssetLoader<Texture>
{
  private static ImmutableHashSet<string> Extensions { get; } = [".ase", ".aseprite"];

  public override bool CanHandle(AssetId id)
  {
    return base.CanHandle(id) && Extensions.Contains(id.Path.Extension);
  }

  public override async Task<Texture> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    var metadata = await context.LoadAsync<AsepriteFile>(context.Path, cancellationToken);

    metadata.Changed += () =>
    {
      // TODO: reload the texture atlas
    };

    throw new NotImplementedException();
  }
}
