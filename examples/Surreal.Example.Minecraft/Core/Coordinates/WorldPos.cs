using System;
using System.Numerics;
using Surreal.Mathematics.Linear;

namespace Minecraft.Core.Coordinates {
  public readonly struct WorldPos : IEquatable<WorldPos> {
    public WorldPos(int x, int y, int z) {
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

    public ChunkPos ChunkPos => new(
        X / World.VoxelsPerChunk.Width,
        Y / World.VoxelsPerChunk.Height,
        Z / World.VoxelsPerChunk.Depth
    );

    public RegionPos RegionPos => new(
        X / World.VoxelsPerRegion.Width,
        Y / World.VoxelsPerRegion.Height,
        Z / World.VoxelsPerRegion.Depth
    );

    public static implicit operator Vector3I(WorldPos pos)   => new(pos.X, pos.Y, pos.Z);
    public static implicit operator Vector3(WorldPos pos)    => new(pos.X, pos.Y, pos.Z);
    public static implicit operator WorldPos(Vector3I point) => new(point.X, point.Y, point.Z);
    public static implicit operator WorldPos(Vector3 vector) => new((int) vector.X, (int) vector.Y, (int) vector.Z);

    public override int GetHashCode() {
      return HashCode.Combine(X, Y, Z);
    }

    public bool Equals(WorldPos other) {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) return false;

      return obj is WorldPos other && Equals(other);
    }

    public static bool operator ==(WorldPos left, WorldPos right) => left.Equals(right);
    public static bool operator !=(WorldPos left, WorldPos right) => !left.Equals(right);

    public override string ToString() => $"Block @ <{X} {Y} {Z}>";
  }
}