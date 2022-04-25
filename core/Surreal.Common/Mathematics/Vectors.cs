﻿using System.Runtime.InteropServices;

namespace Surreal.Mathematics;

#pragma warning disable S1104
#pragma warning disable S2328

/// <summary>An integral point in 2-space.</summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Point2(int X, int Y)
{
  public static readonly Point2 Zero = new(0, 0);
  public static readonly Point2 UnitX = new(1, 0);
  public static readonly Point2 UnitY = new(0, 1);

  public int X = X;
  public int Y = Y;

  public int LengthSquared() => X * X + Y * Y;

  public override string ToString() => $"<{X} {Y}>";

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

  public static implicit operator Point2(Vector2 vector) => new(
    (int) MathF.Floor(vector.X),
    (int) MathF.Floor(vector.Y)
  );
}

/// <summary>An integral point in 3-space.</summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Point3(int X, int Y, int Z)
{
  public static readonly Point3 Zero = new(0, 0, 0);
  public static readonly Point3 UnitX = new(1, 0, 0);
  public static readonly Point3 UnitY = new(0, 1, 0);
  public static readonly Point3 UnitZ = new(0, 0, 1);

  public int X = X;
  public int Y = Y;
  public int Z = Z;

  public override string ToString() => $"<{X} {Y} {Z}>";

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

  public static implicit operator Point3(Vector3 vector) => new(
    (int) MathF.Floor(vector.X),
    (int) MathF.Floor(vector.Y),
    (int) MathF.Floor(vector.Z)
  );
}
