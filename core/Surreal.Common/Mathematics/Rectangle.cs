namespace Surreal.Mathematics;

// TODO: make the coordinate system consistent across the project

/// <summary>A bounding rectangle in 2-space.</summary>
public readonly record struct Rectangle(float Left, float Top, float Right, float Bottom)
{
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

  public PointEnumerator Points => new(TopLeft, Maths.CeilToInt(Width), Maths.CeilToInt(Height));

  public static Rectangle Create(Vector2 center, Vector2 size)
  {
    var left = center.X - size.X / 2f;
    var top = center.Y - size.Y / 2f;
    var right = center.X + size.X / 2f;
    var bottom = center.Y + size.Y / 2f;

    return new Rectangle(left, top, right, bottom);
  }

  public override string ToString()
  {
    return $"Rectangle ({Left}, {Top}, {Right}, {Bottom})";
  }

  /// <summary>Clamps the bounding rect to the given range.</summary>
  public Rectangle Clamp(float minX, float minY, float maxX, float maxY)
  {
    return new Rectangle(
      Left.Clamp(minX, maxX),
      Top.Clamp(minY, maxY),
      Right.Clamp(minX, maxX),
      Bottom.Clamp(minY, maxY)
    );
  }

  public bool Contains(Point2 point)
  {
    return point.X >= Left &&
           point.X <= Right &&
           point.Y >= Top &&
           point.Y <= Bottom;
  }

  public bool Contains(Vector2 vector)
  {
    return vector.X >= Left &&
           vector.X <= Right &&
           vector.Y >= Top &&
           vector.Y <= Bottom;
  }

  /// <summary>Allows enumerating points in a <see cref="Rectangle" />.</summary>
  public struct PointEnumerator : IEnumerable<Point2>, IEnumerator<Point2>
  {
    private readonly Point2 _bottomLeft;
    private readonly int _width;
    private readonly int _height;
    private int _offset;

    public PointEnumerator(Point2 bottomLeft, int width, int height)
    {
      _bottomLeft = bottomLeft;
      _width = width;
      _height = height;
      _offset = -1;

      Reset();
    }

    public Point2 Current => _bottomLeft + new Point2(_offset % _width, _offset / _width);
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      return ++_offset < _width * _height;
    }

    public void Reset()
    {
      _offset = -1;
    }

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



