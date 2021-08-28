using System;
using System.IO;
using Surreal.IO;

namespace Surreal.Mathematics
{
  public struct Seed : IEquatable<Seed>, IBinarySerializable
  {
    public static Seed Default    => default;
    public static Seed Randomized => new(Random.Shared.Next());

    public int Value;

    public Seed(int value)
    {
      Value = value;
    }

    public Seed(string value)
    {
      Value = value.GetHashCode();
    }

    public Random ToRandom()
    {
      if (Value == 0)
      {
        return Random.Shared;
      }

      return new Random(Value);
    }

    public override string ToString() => $"<{Value.ToString()}>";

    public          bool Equals(Seed other)  => Value == other.Value;
    public override bool Equals(object? obj) => obj is Seed other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Seed left, Seed right) => left.Equals(right);
    public static bool operator !=(Seed left, Seed right) => !left.Equals(right);

    void IBinarySerializable.Save(BinaryWriter writer)
    {
      writer.Write(Value);
    }

    void IBinarySerializable.Load(BinaryReader reader)
    {
      Value = reader.ReadInt32();
    }
  }
}
