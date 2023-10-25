using Surreal.Maths;

namespace Surreal.Colors;

/// <summary>
/// A floating-point representation of color.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Color(float R, float G, float B, float A = 1f) : IFromRandom<Color>, IInterpolated<Color>
{
  public static readonly Color Black = new(0, 0, 0);
  public static readonly Color Red = new(1f, 0, 0);
  public static readonly Color Green = new(0, 1f, 0);
  public static readonly Color Blue = new(0, 0, 1f);
  public static readonly Color Yellow = new(1f, 1f, 0);
  public static readonly Color Magenta = new(1f, 0, 1f);
  public static readonly Color Cyan = new(0, 1f, 1f);
  public static readonly Color White = new(1f, 1f, 1f);
  public static readonly Color Clear = new(0, 0, 0, 0);

  public float R = R;
  public float G = G;
  public float B = B;
  public float A = A;

  /// <summary>
  /// Creates a new random color.
  /// </summary>
  public static Color FromRandom(Random random)
  {
    return new Color(
      random.NextFloat(),
      random.NextFloat(),
      random.NextFloat(),
      A: 1f
    );
  }

  /// <summary>
  /// Interpolates between two colors.
  /// </summary>
  public static Color Lerp(Color a, Color b, float t)
  {
    return new Color(
      MathE.Lerp(a.R, b.R, t),
      MathE.Lerp(a.G, b.G, t),
      MathE.Lerp(a.B, b.B, t),
      MathE.Lerp(a.A, b.A, t)
    );
  }

  public bool Equals(Color other)
  {
    return Math.Abs(R - other.R) < float.Epsilon &&
           Math.Abs(G - other.G) < float.Epsilon &&
           Math.Abs(B - other.B) < float.Epsilon &&
           Math.Abs(A - other.A) < float.Epsilon;
  }

  public override string ToString() => $"<{R}, {G}, {B}, {A}>";

  public readonly Color Lighten(float amount) => new(R + amount, G + amount, B + amount);
  public readonly Color Darken(float amount) => new(R - amount, G - amount, B - amount);

  public readonly override int GetHashCode() => HashCode.Combine(R, G, B, A);

  // piecewise operations
  public static Color operator +(Color a, Color b) => new(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
  public static Color operator -(Color a, Color b) => new(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
  public static Color operator *(Color a, Color b) => new(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);
  public static Color operator /(Color a, Color b) => new(a.R / b.R, a.G / b.G, a.B / b.B, a.A / b.A);

  // scalar operations
  public static Color operator *(Color a, float scalar) => new(a.R * scalar, a.G * scalar, a.B * scalar, a.A * scalar);
  public static Color operator /(Color a, float scalar) => new(a.R / scalar, a.G / scalar, a.B / scalar, a.A / scalar);

  // explicit conversions
  public static explicit operator Color(Vector3 vector) => new(vector.X, vector.Y, vector.Z);
  public static explicit operator Color(Vector4 vector) => new(vector.X, vector.Y, vector.Z, vector.W);
  public static explicit operator Vector3(Color color) => new(color.R, color.G, color.B);
  public static explicit operator Vector4(Color color) => new(color.R, color.G, color.B, color.A);

  // implicit conversions
  public static implicit operator Color(Color32 color)
  {
    var r = color.R / 255.0f;
    var g = color.G / 255.0f;
    var b = color.B / 255.0f;
    var a = color.A / 255.0f;

    return new Color(r, g, b, a);
  }
}
