using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear {
  public readonly struct Ray : IEquatable<Ray> {
    public readonly Vector2 Origin;
    public readonly Vector2 Direction;

    public Ray(Vector2 origin, Vector2 direction) {
      Origin    = origin;
      Direction = direction;
    }

    public void Deconstruct(out Vector2 origin, out Vector2 direction) {
      origin    = Origin;
      direction = Direction;
    }

    public bool Equals(Ray other) {
      return Origin.Equals(other.Origin) && Direction.Equals(other.Direction);
    }

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is Ray other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(Origin, Direction);

    public static bool operator ==(Ray left, Ray right) => left.Equals(right);
    public static bool operator !=(Ray left, Ray right) => !left.Equals(right);
  }
}