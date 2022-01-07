using Surreal.Assets;
using Surreal.Mathematics;

namespace Surreal.Graphics.Textures;

/// <summary>Describes a sub-region of a parent <see cref="Texture"/>.</summary>
public readonly record struct TextureRegion(Texture Texture)
{
  public Vector2I Offset { get; init; } = Vector2I.Zero;
  public Vector2I Size   { get; init; } = new(Texture.Width, Texture.Height);

  public int Width  => Size.X;
  public int Height => Size.Y;

  public static implicit operator TextureRegion(Texture texture) => texture.ToRegion();
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="TextureRegion"/>s.</summary>
public sealed class TextureRegionLoader : AssetLoader<TextureRegion>
{
  public override async Task<TextureRegion> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var texture = await context.Manager.LoadAssetAsync<Texture>(context.Path, cancellationToken);

    return texture.ToRegion();
  }
}
