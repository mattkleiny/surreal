using Surreal.Assets;
using Surreal.Mathematics;

namespace Surreal.Graphics.Textures;

/// <summary>Describes a sub-region of a parent <see cref="Texture"/>.</summary>
public readonly record struct TextureRegion(Texture Texture)
{
  public Point2 Offset { get; init; } = Point2.Zero;
  public Point2 Size   { get; init; } = new(Texture.Width, Texture.Height);

  public int X      => Offset.X;
  public int Y      => Offset.Y;
  public int Width  => Size.X;
  public int Height => Size.Y;

  /// <summary>Computes the UV rectangle for the texture region.</summary>
  public Rectangle UV => new(
    (float) Offset.X / Texture.Width,
    (float) Offset.Y / Texture.Height,
    (float) (Offset.X + Size.X) / Texture.Width,
    (float) (Offset.Y + Size.Y) / Texture.Height
  );

  public static implicit operator TextureRegion(Texture texture) => texture.ToRegion();
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="TextureRegion"/>s.</summary>
public sealed class TextureRegionLoader : AssetLoader<TextureRegion>
{
  public override async ValueTask<TextureRegion> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    return await context.LoadDependencyAsync<Texture>(context.Path, cancellationToken);
  }
}
