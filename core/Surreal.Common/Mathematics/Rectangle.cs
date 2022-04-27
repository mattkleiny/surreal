namespace Surreal.Mathematics;

/// <summary>A rectangle in floating point 2-space.</summary>
public record struct Rectangle(float Left, float Top, float Right, float Bottom)
{
  public static Rectangle Empty => new(0, 0, 0, 0);

  public static Rectangle Create(Vector2 center, Vector2 size)
  {
    var left = center.X - size.X / 2f;
    var top = center.Y + size.Y / 2f;
    var right = center.X + size.X / 2f;
    var bottom = center.Y - size.Y / 2f;

    return new Rectangle(left, top, right, bottom);
  }

  public float Left = Left;
  public float Top = Top;
  public float Right = Right;
  public float Bottom = Bottom;

  public float Width  => Right - Left;
  public float Height => Bottom - Top;

  public Vector2 Center => new(Left + Width / 2f, Bottom + Height / 2f);
  public Vector2 Size   => new(Width, Height);

  public Vector2 TopLeft     => new(Left, Top);
  public Vector2 TopRight    => new(Right, Top);
  public Vector2 BottomLeft  => new(Left, Bottom);
  public Vector2 BottomRight => new(Right, Bottom);

  /// <summary>Clamps the box to the given range.</summary>
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
}

/// <summary>A rectangle in integral 2-space.</summary>
public record struct Box(int Left, int Top, int Right, int Bottom)
{
  public static Box Empty => new(0, 0, 0, 0);

  public static Box Create(Point2 center, Point2 size)
  {
    var left = center.X - size.X / 2;
    var top = center.Y + size.Y / 2;
    var right = center.X + size.X / 2;
    var bottom = center.Y - size.Y / 2;

    return new Box(left, top, right, bottom);
  }

  public int Left = Left;
  public int Top = Top;
  public int Right = Right;
  public int Bottom = Bottom;

  public int Width  => Right - Left;
  public int Height => Bottom - Top;

  public Point2 Center => new(Left + Width / 2, Bottom + Height / 2);
  public Point2 Size   => new(Width, Height);

  public Point2 TopLeft     => new(Left, Top);
  public Point2 TopRight    => new(Right, Top);
  public Point2 BottomLeft  => new(Left, Bottom);
  public Point2 BottomRight => new(Right, Bottom);

  /// <summary>Clamps the box to the given range.</summary>
  public Box Clamp(int minX, int minY, int maxX, int maxY) => new(
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
}
