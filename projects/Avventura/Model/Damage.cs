using System;
using Avventura.Model.Effects;

namespace Avventura.Model {
  public struct Damage : IEquatable<Damage> {
    public Damage(int amount, DamageType type) {
      Amount = amount;
      Type   = type;
    }

    public int        Amount { get; }
    public DamageType Type   { get; }

    public static Damage operator +(Damage damage, int scalar) {
      return new Damage(damage.Amount + scalar, damage.Type);
    }

    public static Damage operator -(Damage damage, int scalar) {
      return new Damage(damage.Amount - scalar, damage.Type);
    }

    public          bool Equals(Damage other) => Amount == other.Amount && Type == other.Type;
    public override bool Equals(object? obj)  => obj is Damage other    && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Amount, (int) Type);

    public static bool operator ==(Damage left, Damage right) => left.Equals(right);
    public static bool operator !=(Damage left, Damage right) => !left.Equals(right);
  }
}