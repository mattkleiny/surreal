namespace Surreal.Mathematics;

#pragma warning disable S1104
#pragma warning disable S2328

/// <summary>An integral point in 2-space.</summary>
public struct Point2 : IEquatable<Point2>
{
  public static readonly Point2 Zero  = new(0, 0);
  public static readonly Point2 UnitX = new(1, 0);
  public static readonly Point2 UnitY = new(0, 1);

  public int X;
  public int Y;

  public Point2(int x, int y)
  {
    X = x;
    Y = y;
  }

  public void Deconstruct(out int x, out int y)
  {
    x = X;
    y = Y;
  }

  public override string ToString() => $"<{X} {Y}>";

  public bool Equals(Point2 other) => X == other.X && Y == other.Y;
  public override bool Equals(object? obj) => obj is Point2 other && Equals(other);

  public override int GetHashCode() => HashCode.Combine(X, Y);

  public static bool operator ==(Point2 left, Point2 right) => left.Equals(right);
  public static bool operator !=(Point2 left, Point2 right) => !left.Equals(right);

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
  public static implicit operator Vector2(Point2 self) => new(self.X, self.Y);
  public static implicit operator Point2((int X, int Y) value) => new(value.X, value.Y);
}

/// <summary>An integral point in 3-space.</summary>
public struct Point3 : IEquatable<Point3>
{
  public static readonly Point3 Zero  = new(0, 0, 0);
  public static readonly Point3 UnitX = new(1, 0, 0);
  public static readonly Point3 UnitY = new(0, 1, 0);
  public static readonly Point3 UnitZ = new(0, 0, 1);

  public int X;
  public int Y;
  public int Z;

  public Point3(int x, int y, int z)
  {
    X = x;
    Y = y;
    Z = z;
  }

  public void Deconstruct(out int x, out int y, out int z)
  {
    x = X;
    y = Y;
    z = Z;
  }

  public override string ToString() => $"<{X} {Y} {Z}>";

  public bool Equals(Point3 other) => X == other.X && Y == other.Y && Z == other.Z;
  public override bool Equals(object? obj) => obj is Point3 other && Equals(other);

  public override int GetHashCode() => HashCode.Combine(X, Y, Z);

  public static bool operator ==(Point3 left, Point3 right) => left.Equals(right);
  public static bool operator !=(Point3 left, Point3 right) => !left.Equals(right);

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
  public static implicit operator Vector3(Point3 self) => new(self.X, self.Y, self.Z);
  public static implicit operator Point3((int, int, int) value) => new(value.Item1, value.Item2, value.Item3);
}
