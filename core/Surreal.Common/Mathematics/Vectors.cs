namespace Surreal.Mathematics;

#pragma warning disable S1104
#pragma warning disable S2328

/// <summary>An integral point in 2-space.</summary>
public struct Vector2I : IEquatable<Vector2I>
{
  public static readonly Vector2I Zero  = new(0, 0);
  public static readonly Vector2I UnitX = new(1, 0);
  public static readonly Vector2I UnitY = new(0, 1);

  public int X;
  public int Y;

  public Vector2I(int x, int y)
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

  public          bool Equals(Vector2I other) => X == other.X && Y == other.Y;
  public override bool Equals(object? obj)    => obj is Vector2I other && Equals(other);

  public override int GetHashCode() => HashCode.Combine(X, Y);

  public static bool operator ==(Vector2I left, Vector2I right) => left.Equals(right);
  public static bool operator !=(Vector2I left, Vector2I right) => !left.Equals(right);

  // scalar operations
  public static Vector2I operator +(Vector2I a, int s) => new(a.X + s, a.Y + s);
  public static Vector2I operator -(Vector2I a, int s) => new(a.X - s, a.Y - s);
  public static Vector2I operator *(Vector2I a, int s) => new(a.X * s, a.Y * s);
  public static Vector2I operator /(Vector2I a, int s) => new(a.X / s, a.Y / s);

  // piece-wise operations
  public static Vector2I operator +(Vector2I a, Vector2I b) => new(a.X + b.X, a.Y + b.Y);
  public static Vector2I operator -(Vector2I a, Vector2I b) => new(a.X - b.X, a.Y - b.Y);
  public static Vector2I operator *(Vector2I a, Vector2I b) => new(a.X * b.X, a.Y * b.Y);
  public static Vector2I operator /(Vector2I a, Vector2I b) => new(a.X / b.X, a.Y / b.Y);

  // self-mutation
  public static Vector2I operator -(Vector2I self) => new(-self.X, -self.Y);

  // implicit conversion
  public static implicit operator Vector2(Vector2I self)         => new(self.X, self.Y);
  public static implicit operator Vector2I((int X, int Y) value) => new(value.X, value.Y);
}

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

  public override string ToString() => $"<{X} {Y} {Z}>";

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
