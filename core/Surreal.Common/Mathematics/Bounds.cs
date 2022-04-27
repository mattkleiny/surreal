namespace Surreal.Mathematics;

/// <summary>A bounding rectangle in 2-space.</summary>
public readonly record struct BoundingRect(Vector2 Min, Vector2 Max)
{
  public static BoundingRect Empty => new(Vector2.Zero, Vector2.Zero);

  public BoundingRect(float left, float top, float right, float bottom)
    : this(new Vector2(left, bottom), new Vector2(right, top))
  {
  }

  public static BoundingRect Create(Vector2 center, Vector2 size)
  {
    var left = center.X - size.X / 2f;
    var top = center.Y + size.Y / 2f;
    var right = center.X + size.X / 2f;
    var bottom = center.Y - size.Y / 2f;

    var min = new Vector2(left, bottom);
    var max = new Vector2(right, top);

    return new BoundingRect(min, max);
  }

  public float Left   => Min.X;
  public float Top    => Max.Y;
  public float Right  => Max.X;
  public float Bottom => Min.Y;

  public float Width  => Right - Left;
  public float Height => Top - Bottom;

  public Vector2 Center => new(Left + Width / 2f, Bottom + Height / 2f);
  public Vector2 Size   => new(Width, Height);

  public Vector2 TopLeft     => new(Left, Top);
  public Vector2 TopRight    => new(Right, Top);
  public Vector2 BottomLeft  => new(Left, Bottom);
  public Vector2 BottomRight => new(Right, Bottom);

  public override string ToString()
  {
    return $"BoundingRect ({Left}, {Top}, {Right}, {Bottom})";
  }

  /// <summary>Computes a <see cref="BoundingRect"/> re-centered at the given position.</summary>
  public BoundingRect Recenter(Vector2 center)
  {
    var size = Size;

    return new BoundingRect(
      center.X - size.X / 2f,
      center.Y + size.Y / 2f,
      center.X + size.X / 2f,
      center.Y - size.Y / 2f
    );
  }

  /// <summary>Clamps the bounding rect to the given range.</summary>
  public BoundingRect Clamp(float minX, float minY, float maxX, float maxY) => new(
    Min: new Vector2(Min.X.Clamp(minX, maxX), Min.Y.Clamp(minY, maxY)),
    Max: new Vector2(Max.X.Clamp(minX, maxX), Max.Y.Clamp(minY, maxY))
  );

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
