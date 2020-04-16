using System;
using System.Numerics;
using Surreal.Mathematics.Linear;

namespace Minecraft.Core.Coordinates
{
  public readonly struct ChunkPos : IEquatable<ChunkPos>
  {
    public ChunkPos(int x, int y, int z)
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

    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public static implicit operator Vector3I(ChunkPos pos)     => new Vector3I(pos.X, pos.Y, pos.Z);
    public static implicit operator Vector3(ChunkPos pos)    => new Vector3(pos.X, pos.Y, pos.Z);
    public static implicit operator ChunkPos(Vector3I point)   => new ChunkPos(point.X, point.Y, point.Z);
    public static implicit operator ChunkPos(Vector3 vector) => new ChunkPos((int) vector.X, (int) vector.Y, (int) vector.Z);

    public override int GetHashCode()
    {
      return HashCode.Combine(X, Y, Z);
    }

    public bool Equals(ChunkPos other)
    {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
      return obj is ChunkPos other && Equals(other);
    }

    public static bool operator ==(ChunkPos left, ChunkPos right) => left.Equals(right);
    public static bool operator !=(ChunkPos left, ChunkPos right) => !left.Equals(right);

    public override string ToString() => $"Chunk @ <{X} {Y} {Z}>";
  }
}
