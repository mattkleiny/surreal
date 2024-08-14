using Surreal.Mathematics;

namespace Surreal.Graphics.Textures;

/// <summary>
/// Describes a sub-region of a parent <see cref="Texture" />.
/// </summary>
[DebuggerDisplay("TextureRegion {Texture} at {Offset} with size {Size}")]
public readonly record struct TextureRegion(Texture? Texture)
{
  public static TextureRegion Empty => default;

  /// <summary>
  /// The offset of the texture region into the parent texture.
  /// </summary>
  public Point2 Offset { get; init; } = Point2.Zero;

  /// <summary>
  /// The size of the texture region.
  /// </summary>
  public Point2 Size { get; init; } = new(Texture?.Width ?? 0, Texture?.Height ?? 0);

  public int X => Offset.X;
  public int Y => Offset.Y;
  public int Width => Size.X;
  public int Height => Size.Y;

  /// <summary>
  /// Computes the UV rectangle for the texture region.
  /// </summary>
  public Rectangle UV => new(
    (float)Offset.X / Texture?.Width ?? 1,
    (float)Offset.Y / Texture?.Height ?? 1,
    (float)(Offset.X + Size.X) / Texture?.Width ?? 1,
    (float)(Offset.Y + Size.Y) / Texture?.Height ?? 1
  );

  /// <summary>
  /// Creates a new texture region that is a sub-region of this region.
  /// </summary>
  public TextureRegion Slice(Point2 offset, Point2 size)
  {
    return new TextureRegion(Texture)
    {
      Offset = Offset + offset,
      Size = size
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator TextureRegion(Texture texture) => texture.ToRegion();
}
