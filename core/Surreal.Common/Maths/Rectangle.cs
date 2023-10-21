namespace Surreal.Maths;

// TODO: make the coordinate system consistent across the project

/// <summary>
/// A bounding rectangle in 2-space.
/// </summary>
public readonly record struct Rectangle(float Left, float Top, float Right, float Bottom)
{
  /// <summary>
  /// Creates a rectangle from the given coordinates.
  /// </summary>
  public static Rectangle Create(Vector2 center, Vector2 size)
  {
    var left = center.X - size.X / 2f;
    var top = center.Y - size.Y / 2f;
    var right = center.X + size.X / 2f;
    var bottom = center.Y + size.Y / 2f;

    return new Rectangle(left, top, right, bottom);
  }

  public Rectangle(Vector2 min, Vector2 max)
    : this(min.X, max.Y, max.X, min.Y)
  {
  }

  public static Rectangle Empty => new(Vector2.Zero, Vector2.Zero);

  public Vector2 Min => BottomLeft;
  public Vector2 Max => TopRight;

  public float Width => Right - Left;
  public float Height => Bottom - Top;

  public Vector2 Center => new(Left + Width / 2f, Top + Height / 2f);
  public Vector2 Size => new(Width, Height);

  public Vector2 TopLeft => new(Left, Top);
  public Vector2 TopRight => new(Right, Top);
  public Vector2 BottomLeft => new(Left, Bottom);
  public Vector2 BottomRight => new(Right, Bottom);

  public override string ToString() => $"Rectangle ({Left}, {Top}, {Right}, {Bottom})";

  /// <summary>
  /// Clamps the bounding rect to the given range.
  /// </summary>
  public Rectangle Clamp(float minX, float minY, float maxX, float maxY)
  {
    return new Rectangle(
      Left.Clamp(minX, maxX),
      Top.Clamp(minY, maxY),
      Right.Clamp(minX, maxX),
      Bottom.Clamp(minY, maxY)
    );
  }

  /// <summary>
  /// Determines if the given point is contained within the rectangle.
  /// </summary>
  public bool Contains(Point2 point)
  {
    return point.X >= Left &&
           point.X <= Right &&
           point.Y >= Top &&
           point.Y <= Bottom;
  }

  /// <summary>
  /// Determines if the given point is contained within the rectangle.
  /// </summary>
  public bool Contains(Vector2 vector)
  {
    return vector.X >= Left &&
           vector.X <= Right &&
           vector.Y >= Top &&
           vector.Y <= Bottom;
  }
}
