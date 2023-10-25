using Surreal.Maths;

namespace Surreal.Colors;

/// <summary>
/// A 32-bit representation of color.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Color32(byte R, byte G, byte B, byte A = 255) : IFromRandom<Color32>, IInterpolated<Color32>
{
  public static readonly Color32 Black = new(0, 0, 0);
  public static readonly Color32 Red = new(255, 0, 0);
  public static readonly Color32 Green = new(0, 255, 0);
  public static readonly Color32 Blue = new(0, 0, 255);
  public static readonly Color32 Yellow = new(255, 255, 0);
  public static readonly Color32 Magenta = new(255, 0, 255);
  public static readonly Color32 Cyan = new(0, 255, 255);
  public static readonly Color32 White = new(255, 255, 255);
  public static readonly Color32 Clear = new(0, 0, 0, 0);

  public byte R = R;
  public byte G = G;
  public byte B = B;
  public byte A = A;

  /// <summary>
  /// Creates a new random color.
  /// </summary>
  public static Color32 FromRandom(Random random)
  {
    return new Color32(
      random.NextByte(),
      random.NextByte(),
      random.NextByte(),
      A: 255
    );
  }

  /// <summary>
  /// Interpolates between two colors.
  /// </summary>
  public static Color32 Lerp(Color32 a, Color32 b, float t)
  {
    return new Color32(
      (byte)MathE.Lerp(a.R, b.R, t),
      (byte)MathE.Lerp(a.G, b.G, t),
      (byte)MathE.Lerp(a.B, b.B, t),
      (byte)MathE.Lerp(a.A, b.A, t)
    );
  }

  public override string ToString() => $"<{R}, {G}, {B}, {A}>";

  public readonly Color32 Lighten(byte amount) => new((byte)(R + amount), (byte)(G + amount), (byte)(B + amount));
  public readonly Color32 Darken(byte amount) => new((byte)(R - amount), (byte)(G - amount), (byte)(B - amount));

  // piecewise operations
  public static Color32 operator +(Color32 a, Color32 b) => new((byte)(a.R + b.R), (byte)(a.G + b.G), (byte)(a.B + b.B), (byte)(a.A + b.A));
  public static Color32 operator -(Color32 a, Color32 b) => new((byte)(a.R - b.R), (byte)(a.G - b.G), (byte)(a.B - b.B), (byte)(a.A - b.A));
  public static Color32 operator *(Color32 a, Color32 b) => new((byte)(a.R * b.R), (byte)(a.G * b.G), (byte)(a.B * b.B), (byte)(a.A * b.A));
  public static Color32 operator /(Color32 a, Color32 b) => new((byte)(a.R / b.R), (byte)(a.G / b.G), (byte)(a.B / b.B), (byte)(a.A / b.A));

  // scalar operations
  public static Color32 operator *(Color32 a, int scalar) => new((byte)(a.R * scalar), (byte)(a.G * scalar), (byte)(a.B * scalar), (byte)(a.A * scalar));
  public static Color32 operator /(Color32 a, int scalar) => new((byte)(a.R / scalar), (byte)(a.G / scalar), (byte)(a.B / scalar), (byte)(a.A / scalar));

  // explicit conversions
  public static explicit operator Color32(Point3 vector) => new((byte)vector.X, (byte)vector.Y, (byte)vector.Z);
  public static explicit operator Color32(Point4 vector) => new((byte)vector.X, (byte)vector.Y, (byte)vector.Z, (byte)vector.W);
  public static explicit operator Point3(Color32 color) => new(color.R, color.G, color.B);
  public static explicit operator Point4(Color32 color) => new(color.R, color.G, color.B, color.A);

  // implicit conversions
  public static implicit operator Color32(Color color)
  {
    var r = (byte)(color.R * 255.0f);
    var g = (byte)(color.G * 255.0f);
    var b = (byte)(color.B * 255.0f);
    var a = (byte)(color.A * 255.0f);

    return new Color32(r, g, b, a);
  }
}
