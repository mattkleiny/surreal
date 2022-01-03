using System.Numerics;

namespace Surreal.Mathematics;

/// <summary>A floating-point representation of color.</summary>
public struct Color : IEquatable<Color>
{
  public static readonly Color Black   = new(0, 0, 0);
  public static readonly Color Red     = new(1f, 0, 0);
  public static readonly Color Green   = new(0, 1f, 0);
  public static readonly Color Blue    = new(0, 0, 1f);
  public static readonly Color Yellow  = new(1f, 1f, 0);
  public static readonly Color Magenta = new(1f, 0, 1f);
  public static readonly Color White   = new(1f, 1f, 1f);
  public static readonly Color Clear   = new(0, 0, 0, 0);

  public static Color Lerp(Color a, Color b, float t) => new(
    Maths.Lerp(a.R, b.R, t),
    Maths.Lerp(a.G, b.G, t),
    Maths.Lerp(a.B, b.B, t),
    Maths.Lerp(a.A, b.A, t)
  );

  public Color(float red, float green, float blue, float alpha = 1f)
  {
    R = red;
    G = green;
    B = blue;
    A = alpha;
  }

  public float R { get; set; }
  public float G { get; set; }
  public float B { get; set; }
  public float A { get; set; }

  public override string ToString() => $"<{R}, {G}, {B}, {A}>";

  public bool Equals(Color other)
  {
    return Math.Abs(R - other.R) < float.Epsilon &&
           Math.Abs(G - other.G) < float.Epsilon &&
           Math.Abs(B - other.B) < float.Epsilon &&
           Math.Abs(A - other.A) < float.Epsilon;
  }

  public override bool Equals(object? obj) => obj is Color other && Equals(other);

  public override int GetHashCode() => HashCode.Combine(R, G, B, A);

  public static bool operator ==(Color left, Color right) => left.Equals(right);
  public static bool operator !=(Color left, Color right) => !left.Equals(right);

  public static Color operator +(Color a, Color b)      => new(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
  public static Color operator -(Color a, Color b)      => new(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
  public static Color operator *(Color a, float scalar) => new(a.R * scalar, a.G * scalar, a.B * scalar, a.A * scalar);
  public static Color operator /(Color a, float scalar) => new(a.R / scalar, a.G / scalar, a.B / scalar, a.A / scalar);

  public static explicit operator Color(Vector4 vector) => new(vector.X, vector.Y, vector.Z, vector.W);
  public static explicit operator Color(Vector3 vector) => new(vector.X, vector.Y, vector.Z, 1.0f);
  public static explicit operator Vector4(Color color)  => new(color.R, color.G, color.B, color.A);
  public static explicit operator Vector3(Color color)  => new(color.R, color.G, color.B);
}
