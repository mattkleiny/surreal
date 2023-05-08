using System.Runtime.InteropServices;
using Surreal.Mathematics;

namespace Surreal.Colors;

/// <summary>
/// A 32-bit representation of color.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct ColorB(byte R, byte G, byte B, byte A = 255)
{
  public static readonly ColorB Black = new(0, 0, 0);
  public static readonly ColorB Red = new(255, 0, 0);
  public static readonly ColorB Green = new(0, 255, 0);
  public static readonly ColorB Blue = new(0, 0, 255);
  public static readonly ColorB Yellow = new(255, 255, 0);
  public static readonly ColorB Magenta = new(255, 0, 255);
  public static readonly ColorB Cyan = new(0, 255, 255);
  public static readonly ColorB White = new(255, 255, 255);
  public static readonly ColorB Clear = new(0, 0, 0, 0);

  public byte R = R;
  public byte G = G;
  public byte B = B;
  public byte A = A;

  /// <summary>
  /// Interpolates between two colors.
  /// </summary>
  public static ColorB Lerp(ColorB a, ColorB b, float t)
  {
    return new ColorB(
      (byte)Maths.Lerp(a.R, b.R, t),
      (byte)Maths.Lerp(a.G, b.G, t),
      (byte)Maths.Lerp(a.B, b.B, t),
      (byte)Maths.Lerp(a.A, b.A, t)
    );
  }

  public override string ToString() => $"<{R}, {G}, {B}, {A}>";

  public readonly ColorB Lighten(byte amount) => new((byte)(R + amount), (byte)(G + amount), (byte)(B + amount));
  public readonly ColorB Darken(byte amount) => new((byte)(R - amount), (byte)(G - amount), (byte)(B - amount));

  // piecewise operations
  public static ColorB operator +(ColorB a, ColorB b) => new((byte)(a.R + b.R), (byte)(a.G + b.G), (byte)(a.B + b.B), (byte)(a.A + b.A));
  public static ColorB operator -(ColorB a, ColorB b) => new((byte)(a.R - b.R), (byte)(a.G - b.G), (byte)(a.B - b.B), (byte)(a.A - b.A));
  public static ColorB operator *(ColorB a, ColorB b) => new((byte)(a.R * b.R), (byte)(a.G * b.G), (byte)(a.B * b.B), (byte)(a.A * b.A));
  public static ColorB operator /(ColorB a, ColorB b) => new((byte)(a.R / b.R), (byte)(a.G / b.G), (byte)(a.B / b.B), (byte)(a.A / b.A));

  // scalar operations
  public static ColorB operator *(ColorB a, int scalar) => new((byte)(a.R * scalar), (byte)(a.G * scalar), (byte)(a.B * scalar), (byte)(a.A * scalar));
  public static ColorB operator /(ColorB a, int scalar) => new((byte)(a.R / scalar), (byte)(a.G / scalar), (byte)(a.B / scalar), (byte)(a.A / scalar));

  // explicit conversions
  public static explicit operator ColorB(Point3 vector) => new((byte)vector.X, (byte)vector.Y, (byte)vector.Z);
  public static explicit operator ColorB(Point4 vector) => new((byte)vector.X, (byte)vector.Y, (byte)vector.Z, (byte)vector.W);
  public static explicit operator Point3(ColorB color) => new(color.R, color.G, color.B);
  public static explicit operator Point4(ColorB color) => new(color.R, color.G, color.B, color.A);

  // implicit conversions
  public static implicit operator ColorB(ColorF color)
  {
    var r = (byte)(color.R * 255.0f);
    var g = (byte)(color.G * 255.0f);
    var b = (byte)(color.B * 255.0f);
    var a = (byte)(color.A * 255.0f);

    return new ColorB(r, g, b, a);
  }
}
