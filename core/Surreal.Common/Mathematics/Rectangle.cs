namespace Surreal.Mathematics;

/// <summary>A rectangle in 2-space.</summary>
public readonly record struct Rectangle(float Left, float Top, float Right, float Bottom)
{
  public float Width  => Right - Left;
  public float Height => Bottom - Top;

  public Vector2 Center => new(Left + Width / 2f, Bottom + Height / 2f);
  public Vector2 Size   => new(Width, Height);

  public bool Contains(Point2 point)
  {
    return point.X >= Left &&
           point.X <= Right &&
           point.Y >= Bottom &&
           point.Y <= Top;
  }

  public bool Contains(Vector2 vector)
  {
    return vector.X >= Left &&
           vector.X <= Right &&
           vector.Y >= Bottom &&
           vector.Y <= Top;
  }
}
