using Surreal.Mathematics;

namespace Surreal.Graphics.Textures;

/// <summary>Describes a sub-region of a parent <see cref="Texture"/>.</summary>
[DebuggerDisplay("TextureRegion {Texture} at {Offset} with size {Size}")]
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
