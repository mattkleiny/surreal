namespace Surreal.Mathematics;

/// <summary>
/// An angle, stored as radians with conversions to degrees.
/// </summary>
public readonly record struct Angle(float Radians) : IFromRandom<Angle>, IFromLerp<Angle>
{
  public static Angle Zero => default;
  public static Angle MinValue => new(0f);
  public static Angle MaxValue => new(MathF.PI * 2f);

  /// <summary>
  /// Creates an angle from radians.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Angle FromRadians(float radians)
    => new(radians);

  /// <summary>
  /// Creates an angle from degrees.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Angle FromDegrees(float degrees)
    => new(MathE.DegreesToRadians(degrees));

  /// <summary>
  /// Creates an angle from a random number generator.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Angle FromRandom(Random random)
    => new(MathE.DegreesToRadians(random.NextFloat(0f, 359f)));

  /// <summary>
  /// Interpolates between two angles.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Angle Lerp(Angle a, Angle b, float t)
    => FromRadians(MathE.Lerp(a.Radians, b.Radians, t));

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
  /// Converts the angle to degrees.
  /// </summary>
  public float Degrees => MathE.RadiansToDegrees(Radians);

  public int CompareTo(Angle other)
  {
    return Radians.CompareTo(other.Radians);
  }

  public override string ToString() => $"{Degrees:F}°";

  // comparisons
  public static bool operator <(Angle left, Angle right) => left.Radians < right.Radians;
  public static bool operator >(Angle left, Angle right) => left.Radians > right.Radians;
  public static bool operator <=(Angle left, Angle right) => left.Radians <= right.Radians;
  public static bool operator >=(Angle left, Angle right) => left.Radians >= right.Radians;

  // arithmetic operators
  public static Angle operator +(Angle left, Angle right) => new(left.Radians + right.Radians);
  public static Angle operator -(Angle left, Angle right) => new(left.Radians - right.Radians);
  public static Angle operator *(Angle angle, float scalar) => new(angle.Radians * scalar);
  public static Angle operator /(Angle angle, float scalar) => new(angle.Radians / scalar);
  public static Angle operator *(float scalar, Angle angle) => new(angle.Radians * scalar);
  public static Angle operator /(float scalar, Angle angle) => new(angle.Radians / scalar);

  // implicit conversions
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator float(Angle angle) => angle.Radians;
}
