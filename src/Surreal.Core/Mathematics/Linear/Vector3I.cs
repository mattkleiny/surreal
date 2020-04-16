using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear
{
  public struct Vector3I : IEquatable<Vector3I>
  {
    public static readonly Vector3I Zero  = new Vector3I(0, 0, 0);
    public static readonly Vector3I UnitX = new Vector3I(1, 0, 0);
    public static readonly Vector3I UnitY = new Vector3I(0, 1, 0);
    public static readonly Vector3I UnitZ = new Vector3I(0, 0, 1);

    public int X;
    public int Y;
    public int Z;

    public Vector3I(int x, int y, int z)
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

    // scalar operations
    public static Vector3I operator +(Vector3I a, int s) => new Vector3I(a.X + s, a.Y + s, a.Z + s);
    public static Vector3I operator -(Vector3I a, int s) => new Vector3I(a.X - s, a.Y - s, a.Z - s);
    public static Vector3I operator *(Vector3I a, int s) => new Vector3I(a.X * s, a.Y * s, a.Z * s);
    public static Vector3I operator /(Vector3I a, int s) => new Vector3I(a.X / s, a.Y / s, a.Z / s);

    // piece-wise operations
    public static Vector3I operator +(Vector3I a, Vector3I b) => new Vector3I(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3I operator -(Vector3I a, Vector3I b) => new Vector3I(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3I operator *(Vector3I a, Vector3I b) => new Vector3I(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    public static Vector3I operator /(Vector3I a, Vector3I b) => new Vector3I(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

    // self-mutation
    public static Vector3I operator -(Vector3I self) => new Vector3I(-self.X, -self.Y, -self.Z);

    // implicit conversion
    public static implicit operator Vector3(Vector3I self)          => new Vector3(self.X, self.Y, self.Z);
    public static implicit operator Vector3I((int, int, int) value) => new Vector3I(value.Item1, value.Item2, value.Item3);

    public bool Equals(Vector3I other)
    {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is Vector3I other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public static bool operator ==(Vector3I left, Vector3I right) => left.Equals(right);
    public static bool operator !=(Vector3I left, Vector3I right) => !left.Equals(right);

    public override string ToString() => $"<{X} {Y} {Z}>";
  }
}
