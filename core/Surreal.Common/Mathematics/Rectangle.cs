namespace Surreal.Mathematics;

/// <summary>A bounding rectangle in 2-space.</summary>
public readonly record struct Rectangle(float Left, float Top, float Right, float Bottom)
{
  public static Rectangle Empty => new(Vector2.Zero, Vector2.Zero);

  public Rectangle(Vector2 min, Vector2 max)
    : this(min.X, max.Y, max.X, min.Y)
  {
  }

  public static Rectangle Create(Vector2 center, Vector2 size)
  {
    var left = center.X - size.X / 2f;
    var top = center.Y + size.Y / 2f;
    var right = center.X + size.X / 2f;
    var bottom = center.Y - size.Y / 2f;

    return new Rectangle(left, top, right, bottom);
  }

  public Vector2 Min => BottomLeft;
  public Vector2 Max => TopRight;

  public float Width  => Right - Left;
  public float Height => Top - Bottom;

  public Vector2 Center => new(Left + Width / 2f, Bottom + Height / 2f);
  public Vector2 Size   => new(Width, Height);

  public Vector2 TopLeft     => new(Left, Top);
  public Vector2 TopRight    => new(Right, Top);
  public Vector2 BottomLeft  => new(Left, Bottom);
  public Vector2 BottomRight => new(Right, Bottom);

  public PointEnumerator Points => new(Maths.CeilToInt(Width), Maths.CeilToInt(Height));

  public override string ToString()
  {
    return $"Rectangle ({Left}, {Top}, {Right}, {Bottom})";
  }

  /// <summary>Clamps the bounding rect to the given range.</summary>
  public Rectangle Clamp(float minX, float minY, float maxX, float maxY) => new(
    Left: Left.Clamp(minX, maxX),
    Top: Top.Clamp(minY, maxY),
    Right: Right.Clamp(minX, maxX),
    Bottom: Bottom.Clamp(minY, maxY)
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

  /// <summary>Allows enumerating points in a <see cref="Rectangle"/>.</summary>
  public struct PointEnumerator : IEnumerable<Point2>, IEnumerator<Point2>
  {
    private readonly int width;
    private readonly int height;
    private int offset;

    public PointEnumerator(int width, int height)
      : this()
    {
      this.width  = width;
      this.height = height;

      Reset();
    }

    public Point2      Current => new(offset % width, offset / width);
    object IEnumerator.Current => Current;

    public bool MoveNext() => ++offset < width * height;
    public void Reset() => offset = -1;

    public void Dispose()
    {
      // no-op
    }

    public PointEnumerator GetEnumerator()
    {
      return this;
    }

    IEnumerator<Point2> IEnumerable<Point2>.GetEnumerator()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
