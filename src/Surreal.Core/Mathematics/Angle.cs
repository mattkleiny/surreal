using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static Surreal.Mathematics.Maths;

namespace Surreal.Mathematics {
  // TODO: implement signed angles
  
  public readonly struct Angle : IEquatable<Angle>, IComparable<Angle> {
    public static Angle Zero => default;

    public static Angle FromRadians(float radians) => new Angle(radians);
    public static Angle FromDegrees(float degrees) => new Angle(DegreesToRadians(degrees));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle Lerp(Angle a, Angle b, float t) {
      return FromRadians(Maths.Lerp(a.Radians, b.Radians, t));
    }

    public static Angle Between(Vector2 a, Vector2 b) {
      var dot = Vector2.Dot(a, b);

      var mag1 = a.Length();
      var mag2 = b.Length();

      var radians = MathF.Acos(dot / (mag1 * mag2));

      return FromRadians(radians);
    }

    public static Angle Between(Vector3 a, Vector3 b) {
      var dot = Vector3.Dot(a, b);

      var mag1 = a.Length();
      var mag2 = b.Length();

      var radians = MathF.Acos(dot / (mag1 * mag2));

      return FromRadians(radians);
    }

    private Angle(float radians) {
      Radians = radians;
    }

    public float Radians { get; }
    public float Degrees => RadiansToDegrees(Radians);

    public override string ToString() => $"{Degrees:F}°";

    public          bool Equals(Angle other) => Radians.Equals(other.Radians);
    public override bool Equals(object? obj) => obj is Angle other && Equals(other);

    public int CompareTo(Angle other) => Radians.CompareTo(other.Radians);

    public override int GetHashCode() => Radians.GetHashCode();

    public static bool operator ==(Angle left, Angle right) => left.Equals(right);
    public static bool operator !=(Angle left, Angle right) => !left.Equals(right);
    public static bool operator <(Angle left, Angle right)  => left.CompareTo(right) < 0;
    public static bool operator >(Angle left, Angle right)  => left.CompareTo(right) > 0;
    public static bool operator <=(Angle left, Angle right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Angle left, Angle right) => left.CompareTo(right) >= 0;
  }
}