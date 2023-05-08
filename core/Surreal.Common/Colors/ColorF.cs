using System.Runtime.InteropServices;
using Surreal.Mathematics;

namespace Surreal.Colors;

/// <summary>
/// A floating-point representation of color.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct ColorF(float R, float G, float B, float A = 1f)
{
  public static readonly ColorF Black = new(0, 0, 0);
  public static readonly ColorF Red = new(1f, 0, 0);
  public static readonly ColorF Green = new(0, 1f, 0);
  public static readonly ColorF Blue = new(0, 0, 1f);
  public static readonly ColorF Yellow = new(1f, 1f, 0);
  public static readonly ColorF Magenta = new(1f, 0, 1f);
  public static readonly ColorF Cyan = new(0, 1f, 1f);
  public static readonly ColorF White = new(1f, 1f, 1f);
  public static readonly ColorF Clear = new(0, 0, 0, 0);

  public float R = R;
  public float G = G;
  public float B = B;
  public float A = A;

  /// <summary>
  /// Interpolates between two colors.
  /// </summary>
  public static ColorF Lerp(ColorF a, ColorF b, float t)
  {
    return new ColorF(
      Maths.Lerp(a.R, b.R, t),
      Maths.Lerp(a.G, b.G, t),
      Maths.Lerp(a.B, b.B, t),
      Maths.Lerp(a.A, b.A, t)
    );
  }

  public bool Equals(ColorF other)
  {
    return Math.Abs(R - other.R) < float.Epsilon &&
           Math.Abs(G - other.G) < float.Epsilon &&
           Math.Abs(B - other.B) < float.Epsilon &&
           Math.Abs(A - other.A) < float.Epsilon;
  }

  public override string ToString() => $"<{R}, {G}, {B}, {A}>";

  public readonly ColorF Lighten(float amount) => new(R + amount, G + amount, B + amount);
  public readonly ColorF Darken(float amount) => new(R - amount, G - amount, B - amount);

  public readonly override int GetHashCode() => HashCode.Combine(R, G, B, A);

  // piecewise operations
  public static ColorF operator +(ColorF a, ColorF b) => new(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
  public static ColorF operator -(ColorF a, ColorF b) => new(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
  public static ColorF operator *(ColorF a, ColorF b) => new(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);
  public static ColorF operator /(ColorF a, ColorF b) => new(a.R / b.R, a.G / b.G, a.B / b.B, a.A / b.A);

  // scalar operations
  public static ColorF operator *(ColorF a, float scalar) => new(a.R * scalar, a.G * scalar, a.B * scalar, a.A * scalar);
  public static ColorF operator /(ColorF a, float scalar) => new(a.R / scalar, a.G / scalar, a.B / scalar, a.A / scalar);

  // explicit conversions
  public static explicit operator ColorF(Vector3 vector) => new(vector.X, vector.Y, vector.Z);
  public static explicit operator ColorF(Vector4 vector) => new(vector.X, vector.Y, vector.Z, vector.W);
  public static explicit operator Vector3(ColorF color) => new(color.R, color.G, color.B);
  public static explicit operator Vector4(ColorF color) => new(color.R, color.G, color.B, color.A);

  // implicit conversions
  public static implicit operator ColorF(ColorB color)
  {
    var r = color.R / 255.0f;
    var g = color.G / 255.0f;
    var b = color.B / 255.0f;
    var a = color.A / 255.0f;

    return new ColorF(r, g, b, a);
  }
}
