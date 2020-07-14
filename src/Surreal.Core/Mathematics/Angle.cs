using System;
using static Surreal.Mathematics.Maths;

namespace Surreal.Mathematics {
  public readonly struct Angle : IEquatable<Angle>, IComparable<Angle>, IComparable {
    public static Angle Zero                       => default;
    public static Angle FromRadians(float radians) => new Angle(radians);
    public static Angle FromDegrees(float degrees) => new Angle(DegreesToRadians(degrees));

    private Angle(float radians) {
      Radians = radians;
    }

    public float Radians { get; }
    public float Degrees => RadiansToDegrees(Radians);

    public override string ToString() => $"{Radians:F} radians";

    public          bool Equals(Angle other) => Radians.Equals(other.Radians);
    public override bool Equals(object? obj) => obj is Angle other && Equals(other);

    public int CompareTo(Angle other) {
      return Radians.CompareTo(other.Radians);
    }

    public int CompareTo(object? obj) {
      if (ReferenceEquals(null, obj)) return 1;

      return obj is Angle other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Angle)}");
    }

    public override int GetHashCode() => Radians.GetHashCode();

    public static bool operator ==(Angle left, Angle right) => left.Equals(right);
    public static bool operator !=(Angle left, Angle right) => !left.Equals(right);

    public static bool operator <(Angle left, Angle right)  => left.CompareTo(right) < 0;
    public static bool operator >(Angle left, Angle right)  => left.CompareTo(right) > 0;
    public static bool operator <=(Angle left, Angle right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Angle left, Angle right) => left.CompareTo(right) >= 0;
  }
}