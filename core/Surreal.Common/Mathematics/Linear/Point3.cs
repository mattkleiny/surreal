using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear {
  public struct Point3 : IEquatable<Point3> {
    public static readonly Point3 Zero  = new(0, 0, 0);
    public static readonly Point3 UnitX = new(1, 0, 0);
    public static readonly Point3 UnitY = new(0, 1, 0);
    public static readonly Point3 UnitZ = new(0, 0, 1);

    public int X;
    public int Y;
    public int Z;

    public Point3(int x, int y, int z) {
      X = x;
      Y = y;
      Z = z;
    }

    public void Deconstruct(out int x, out int y, out int z) {
      x = X;
      y = Y;
      z = Z;
    }

    // scalar operations
    public static Point3 operator +(Point3 a, int s) => new(a.X + s, a.Y + s, a.Z + s);
    public static Point3 operator -(Point3 a, int s) => new(a.X - s, a.Y - s, a.Z - s);
    public static Point3 operator *(Point3 a, int s) => new(a.X * s, a.Y * s, a.Z * s);
    public static Point3 operator /(Point3 a, int s) => new(a.X / s, a.Y / s, a.Z / s);

    // piece-wise operations
    public static Point3 operator +(Point3 a, Point3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Point3 operator -(Point3 a, Point3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Point3 operator *(Point3 a, Point3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    public static Point3 operator /(Point3 a, Point3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

    // self-mutation
    public static Point3 operator -(Point3 self) => new(-self.X, -self.Y, -self.Z);

    // implicit conversion
    public static implicit operator Vector3(Point3 self)          => new(self.X, self.Y, self.Z);
    public static implicit operator Point3((int, int, int) value) => new(value.Item1, value.Item2, value.Item3);

    public bool Equals(Point3 other) {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is Point3 other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public static bool operator ==(Point3 left, Point3 right) => left.Equals(right);
    public static bool operator !=(Point3 left, Point3 right) => !left.Equals(right);

    public override string ToString() => $"<{X.ToString()} {Y.ToString()} {Z.ToString()}>";
  }
}