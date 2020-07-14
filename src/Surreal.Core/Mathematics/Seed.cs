using System;
using System.Diagnostics;

namespace Surreal.Mathematics {
  [DebuggerDisplay("Seed {Value}")]
  public readonly struct Seed : IEquatable<Seed> {
    public static Seed NewRandomized() => new Seed(Maths.Random.Next());

    public readonly int Value;

    public Seed(int value) {
      Value = value;
    }

    public Seed(string value) {
      Value = value.GetHashCode();
    }

    public Random ToRandom() {
      if (Value == 0) {
        return new Random(Maths.Random.Next());
      }

      return new Random(Value);
    }

    public          bool Equals(Seed other) => Value == other.Value;
    public override bool Equals(object obj) => obj is Seed other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Seed left, Seed right) => left.Equals(right);
    public static bool operator !=(Seed left, Seed right) => !left.Equals(right);

    public static implicit operator int(Seed seed) => seed.Value;
  }
}