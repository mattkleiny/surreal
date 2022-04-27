namespace Surreal.Mathematics;

/// <summary>A rectangle in integral 2-space.</summary>
public record struct Rectangle(int Left, int Top, int Right, int Bottom)
{
  public static Rectangle Empty => new(0, 0, 0, 0);

  public static Rectangle Create(Point2 center, Point2 size)
  {
    var left = center.X - size.X / 2;
    var top = center.Y + size.Y / 2;
    var right = center.X + size.X / 2;
    var bottom = center.Y - size.Y / 2;

    return new Rectangle(left, top, right, bottom);
  }

  public int Left = Left;
  public int Top = Top;
  public int Right = Right;
  public int Bottom = Bottom;

  public int Width  => Right - Left;
  public int Height => Bottom - Top;

  public Point2 Min    => BottomLeft;
  public Point2 Max    => TopRight;
  public Point2 Center => new(Left + Width / 2, Bottom + Height / 2);
  public Point2 Size   => new(Width, Height);

  public Point2 TopLeft     => new(Left, Top);
  public Point2 TopRight    => new(Right, Top);
  public Point2 BottomLeft  => new(Left, Bottom);
  public Point2 BottomRight => new(Right, Bottom);

  /// <summary>Enumerates all discrete points in the rectangle.</summary>
  public PointEnumerator Points => new(Width, Height);

  /// <summary>Clamps the rectangle to the given range.</summary>
  public Rectangle Clamp(int minX, int minY, int maxX, int maxY) => new(
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
