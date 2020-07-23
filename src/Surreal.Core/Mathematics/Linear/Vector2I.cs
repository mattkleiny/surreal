using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear {
  public struct Vector2I : IEquatable<Vector2I> {
    public static readonly Vector2I Zero  = new Vector2I(0, 0);
    public static readonly Vector2I UnitX = new Vector2I(1, 0);
    public static readonly Vector2I UnitY = new Vector2I(0, 1);

    public int X;
    public int Y;

    public Vector2I(int x, int y) {
      X = x;
      Y = y;
    }

    public void Deconstruct(out int x, out int y) {
      x = X;
      y = Y;
    }

    // scalar operations
    public static Vector2I operator +(Vector2I a, int s) => new Vector2I(a.X + s, a.Y + s);
    public static Vector2I operator -(Vector2I a, int s) => new Vector2I(a.X - s, a.Y - s);
    public static Vector2I operator *(Vector2I a, int s) => new Vector2I(a.X * s, a.Y * s);
    public static Vector2I operator /(Vector2I a, int s) => new Vector2I(a.X / s, a.Y / s);

    // piece-wise operations
    public static Vector2I operator +(Vector2I a, Vector2I b) => new Vector2I(a.X + b.X, a.Y + b.Y);
    public static Vector2I operator -(Vector2I a, Vector2I b) => new Vector2I(a.X - b.X, a.Y - b.Y);
    public static Vector2I operator *(Vector2I a, Vector2I b) => new Vector2I(a.X * b.X, a.Y * b.Y);
    public static Vector2I operator /(Vector2I a, Vector2I b) => new Vector2I(a.X / b.X, a.Y / b.Y);

    // self-mutation
    public static Vector2I operator -(Vector2I self) => new Vector2I(-self.X, -self.Y);

    // implicit conversion
    public static implicit operator Vector2(Vector2I self)         => new Vector2(self.X, self.Y);
    public static implicit operator Vector2I((int X, int Y) value) => new Vector2I(value.X, value.Y);

    public bool Equals(Vector2I other) {
      return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is Vector2I other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Vector2I left, Vector2I right) => left.Equals(right);
    public static bool operator !=(Vector2I left, Vector2I right) => !left.Equals(right);

    public override string ToString() => $"<{X} {Y}>";
  }
}