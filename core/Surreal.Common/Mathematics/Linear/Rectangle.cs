using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear
{
  public readonly struct Rectangle : IEquatable<Rectangle>
  {
    public Rectangle(Vector2 min, Vector2 max)
    {
      Left   = min.X;
      Top    = max.Y;
      Right  = max.X;
      Bottom = min.Y;
    }

    public Rectangle(float left, float top, float right, float bottom)
    {
      Left   = left;
      Top    = top;
      Right  = right;
      Bottom = bottom;
    }

    public void Deconstruct(out float left, out float top, out float right, out float bottom)
    {
      left   = Left;
      top    = Top;
      right  = Right;
      bottom = Bottom;
    }

    public float Left   { get; }
    public float Top    { get; }
    public float Right  { get; }
    public float Bottom { get; }

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

    public override string ToString() => $"<{Left.ToString()}, {Top.ToString()}, {Right.ToString()}, {Bottom.ToString()}>";

    public bool Equals(Rectangle other)
    {
      return Left.Equals(other.Left) &&
             Top.Equals(other.Top) &&
             Right.Equals(other.Right) &&
             Bottom.Equals(other.Bottom);
    }

    public override bool Equals(object? obj)
    {
      return obj is Rectangle other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

    public static bool operator ==(Rectangle left, Rectangle right) => left.Equals(right);
    public static bool operator !=(Rectangle left, Rectangle right) => !left.Equals(right);
  }
}