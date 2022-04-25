using Surreal.Assets;
using Surreal.Mathematics;

namespace Surreal.Graphics.Textures;

/// <summary>Describes a sub-region of a parent <see cref="Texture"/>.</summary>
public readonly record struct TextureRegion(Texture Texture)
{
  public Point2 Offset { get; init; } = Point2.Zero;
  public Point2 Size   { get; init; } = new(Texture.Width, Texture.Height);

  public int Width  => Size.X;
  public int Height => Size.Y;

  public static implicit operator TextureRegion(Texture texture) => texture.ToRegion();
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="TextureRegion"/>s.</summary>
public sealed class TextureRegionLoader : AssetLoader<TextureRegion>
{
  public override async ValueTask<TextureRegion> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var texture = await context.Manager.LoadAsset<Texture>(context.Path);

    return texture.ToRegion();
  }
}
