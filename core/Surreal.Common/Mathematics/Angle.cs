using static Surreal.Mathematics.Maths;

namespace Surreal.Mathematics;

/// <summary>
/// An angle, stored as radians with conversions to degrees.
/// </summary>
public readonly record struct Angle(float Radians) : IComparable<Angle>
{
  public float Degrees => RadiansToDegrees(Radians);

  public static Angle Zero => default;

  public int CompareTo(Angle other)
  {
    return Radians.CompareTo(other.Radians);
  }

  public static Angle FromRadians(float radians)
  {
    return new Angle(radians);
  }

  public static Angle FromDegrees(float degrees)
  {
    return new Angle(DegreesToRadians(degrees));
  }

  public static Angle Lerp(Angle a, Angle b, float t)
  {
    return FromRadians(Maths.Lerp(a.Radians, b.Radians, t));
  }

  public static Angle Between(Vector2 a, Vector2 b)
  {
    var dot = Vector2.Dot(a, b);

    var mag1 = a.Length();
    var mag2 = b.Length();

    var radians = MathF.Acos(dot / (mag1 * mag2));

    return FromRadians(radians);
  }

  public static Angle Between(Vector3 a, Vector3 b)
  {
    var dot = Vector3.Dot(a, b);

    var mag1 = a.Length();
    var mag2 = b.Length();

    var radians = MathF.Acos(dot / (mag1 * mag2));

    return FromRadians(radians);
  }

  public Span<Vector2> SweepArc(float radius, Span<Vector2> storage)
  {
    var theta = 0f;
    var delta = Radians / storage.Length;

    for (var i = 0; i < storage.Length; i++)
    {
      theta += delta;

      var x = radius * MathF.Cos(theta);
      var y = radius * MathF.Sin(theta);

      storage[i] = new Vector2(x, y);
    }

    return storage;
  }

  public override string ToString()
  {
    return $"{Degrees:F}°";
  }

  public static bool operator <(Angle left, Angle right)
  {
    return left.Radians < right.Radians;
  }

  public static bool operator >(Angle left, Angle right)
  {
    return left.Radians > right.Radians;
  }

  public static bool operator <=(Angle left, Angle right)
  {
    return left.Radians <= right.Radians;
  }

  public static bool operator >=(Angle left, Angle right)
  {
    return left.Radians >= right.Radians;
  }
}
