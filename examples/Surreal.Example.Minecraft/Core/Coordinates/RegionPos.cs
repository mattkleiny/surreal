using System;
using System.Numerics;
using Surreal.Mathematics.Linear;

namespace Minecraft.Core.Coordinates {
  public readonly struct RegionPos : IEquatable<RegionPos> {
    public RegionPos(int x, int y, int z) {
      X = x;
      Y = y;
      Z = z;
    }

    public void Deconstruct(out int x, out int y, out int z) {
      x = X;
      y = Y;
      z = Z;
    }

    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public static implicit operator Vector3I(RegionPos pos)   => new(pos.X, pos.Y, pos.Z);
    public static implicit operator Vector3(RegionPos pos)    => new(pos.X, pos.Y, pos.Z);
    public static implicit operator RegionPos(Vector3I point) => new(point.X, point.Y, point.Z);
    public static implicit operator RegionPos(Vector3 vector) => new((int) vector.X, (int) vector.Y, (int) vector.Z);

    public override int GetHashCode() {
      return HashCode.Combine(X, Y, Z);
    }

    public bool Equals(RegionPos other) {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) return false;

      return obj is RegionPos other && Equals(other);
    }

    public static bool operator ==(RegionPos left, RegionPos right) => left.Equals(right);
    public static bool operator !=(RegionPos left, RegionPos right) => !left.Equals(right);

    public override string ToString() => $"Region @ <{X} {Y} {Z}>";
  }
}