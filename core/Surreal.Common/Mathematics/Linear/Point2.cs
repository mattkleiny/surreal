using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear {
  public struct Point2 : IEquatable<Point2> {
    public static readonly Point2 Zero  = new(0, 0);
    public static readonly Point2 UnitX = new(1, 0);
    public static readonly Point2 UnitY = new(0, 1);

    public int X;
    public int Y;

    public Point2(int x, int y) {
      X = x;
      Y = y;
    }

    public void Deconstruct(out int x, out int y) {
      x = X;
      y = Y;
    }

    // scalar operations
    public static Point2 operator +(Point2 a, int s) => new(a.X + s, a.Y + s);
    public static Point2 operator -(Point2 a, int s) => new(a.X - s, a.Y - s);
    public static Point2 operator *(Point2 a, int s) => new(a.X * s, a.Y * s);
    public static Point2 operator /(Point2 a, int s) => new(a.X / s, a.Y / s);

    // piece-wise operations
    public static Point2 operator +(Point2 a, Point2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Point2 operator -(Point2 a, Point2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Point2 operator *(Point2 a, Point2 b) => new(a.X * b.X, a.Y * b.Y);
    public static Point2 operator /(Point2 a, Point2 b) => new(a.X / b.X, a.Y / b.Y);

    // self-mutation
    public static Point2 operator -(Point2 self) => new(-self.X, -self.Y);

    // implicit conversion
    public static implicit operator Vector2(Point2 self)         => new(self.X, self.Y);
    public static implicit operator Point2((int X, int Y) value) => new(value.X, value.Y);

    public bool Equals(Point2 other) {
      return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is Point2 other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Point2 left, Point2 right) => left.Equals(right);
    public static bool operator !=(Point2 left, Point2 right) => !left.Equals(right);

    public override string ToString() => $"<{X} {Y}>";
  }
}