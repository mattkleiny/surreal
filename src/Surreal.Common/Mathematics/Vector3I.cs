using System.Numerics;

namespace Surreal.Mathematics;

/// <summary>An integral point in 3-space.</summary>
public struct Vector3I : IEquatable<Vector3I>
{
  public static readonly Vector3I Zero  = new(0, 0, 0);
  public static readonly Vector3I UnitX = new(1, 0, 0);
  public static readonly Vector3I UnitY = new(0, 1, 0);
  public static readonly Vector3I UnitZ = new(0, 0, 1);

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

  public override string ToString() => $"<{X.ToString()} {Y.ToString()} {Z.ToString()}>";

  public          bool Equals(Vector3I other) => X == other.X && Y == other.Y && Z == other.Z;
  public override bool Equals(object? obj)    => obj is Vector3I other && Equals(other);

  public override int GetHashCode() => HashCode.Combine(X, Y, Z);

  public static bool operator ==(Vector3I left, Vector3I right) => left.Equals(right);
  public static bool operator !=(Vector3I left, Vector3I right) => !left.Equals(right);

  // scalar operations
  public static Vector3I operator +(Vector3I a, int s) => new(a.X + s, a.Y + s, a.Z + s);
  public static Vector3I operator -(Vector3I a, int s) => new(a.X - s, a.Y - s, a.Z - s);
  public static Vector3I operator *(Vector3I a, int s) => new(a.X * s, a.Y * s, a.Z * s);
  public static Vector3I operator /(Vector3I a, int s) => new(a.X / s, a.Y / s, a.Z / s);

  // piece-wise operations
  public static Vector3I operator +(Vector3I a, Vector3I b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
  public static Vector3I operator -(Vector3I a, Vector3I b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
  public static Vector3I operator *(Vector3I a, Vector3I b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
  public static Vector3I operator /(Vector3I a, Vector3I b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

  // self-mutation
  public static Vector3I operator -(Vector3I self) => new(-self.X, -self.Y, -self.Z);

  // implicit conversion
  public static implicit operator Vector3(Vector3I self)          => new(self.X, self.Y, self.Z);
  public static implicit operator Vector3I((int, int, int) value) => new(value.Item1, value.Item2, value.Item3);
}
