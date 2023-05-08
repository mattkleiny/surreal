namespace Surreal.Mathematics;

/// <summary>
/// An angle, stored as radians with conversions to degrees.
/// </summary>
public readonly record struct Angle(float Radians) : IComparable<Angle>
{
  public static Angle Zero => default;

  /// <summary>
  /// Creates an angle from radians.
  /// </summary>
  public static Angle FromRadians(float radians)
  {
    return new Angle(radians);
  }

  /// <summary>
  /// Creates an angle from degrees.
  /// </summary>
  public static Angle FromDegrees(float degrees)
  {
    return new Angle(Maths.DegreesToRadians(degrees));
  }

  /// <summary>
  /// Returns the angle between two vectors.
  /// </summary>
  public static Angle Between(Vector2 a, Vector2 b)
  {
    var dot = Vector2.Dot(a, b);

    var mag1 = a.Length();
    var mag2 = b.Length();

    var radians = MathF.Acos(dot / (mag1 * mag2));

    return FromRadians(radians);
  }

  /// <summary>
  /// Returns the angle between two vectors.
  /// </summary>
  public static Angle Between(Vector3 a, Vector3 b)
  {
    var dot = Vector3.Dot(a, b);

    var mag1 = a.Length();
    var mag2 = b.Length();

    var radians = MathF.Acos(dot / (mag1 * mag2));

    return FromRadians(radians);
  }

  /// <summary>
  /// Sweeps a circle of the given radius around the origin, returning the points in the given storage.
  /// </summary>
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

  /// <summary>
  /// Converts the angle to degrees.
  /// </summary>
  public float Degrees => Maths.RadiansToDegrees(Radians);

  /// <summary>
  /// Interpolates between two angles.
  /// </summary>
  public static Angle Lerp(Angle a, Angle b, float t)
  {
    return FromRadians(Maths.Lerp(a.Radians, b.Radians, t));
  }

  public int CompareTo(Angle other)
  {
    return Radians.CompareTo(other.Radians);
  }

  public override string ToString() => $"{Degrees:F}°";

  public static bool operator <(Angle left, Angle right) => left.Radians < right.Radians;
  public static bool operator >(Angle left, Angle right) => left.Radians > right.Radians;
  public static bool operator <=(Angle left, Angle right) => left.Radians <= right.Radians;
  public static bool operator >=(Angle left, Angle right) => left.Radians >= right.Radians;
}
