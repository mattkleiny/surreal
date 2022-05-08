using System.Runtime.InteropServices;

namespace Surreal.Mathematics;

#pragma warning disable S1104
#pragma warning disable S2328

/// <summary>A floating-point representation of color.</summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Color(float R, float G, float B, float A = 1f)
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

  public static Color Lerp(Color a, Color b, float t) => new(
    Maths.Lerp(a.R, b.R, t),
    Maths.Lerp(a.G, b.G, t),
    Maths.Lerp(a.B, b.B, t),
    Maths.Lerp(a.A, b.A, t)
  );

  public float R = R;
  public float G = G;
  public float B = B;
  public float A = A;

  public readonly Color Lighten(float amount) => new(R + amount, G + amount, B + amount);
  public readonly Color Darken(float amount) => new(R - amount, G - amount, B - amount);

  public override string ToString() => $"<{R}, {G}, {B}, {A}>";

  public bool Equals(Color other)
  {
    return Math.Abs(R - other.R) < float.Epsilon &&
           Math.Abs(G - other.G) < float.Epsilon &&
           Math.Abs(B - other.B) < float.Epsilon &&
           Math.Abs(A - other.A) < float.Epsilon;
  }

  public readonly override int GetHashCode()
  {
    return HashCode.Combine(R, G, B, A);
  }

  public static Color operator +(Color a, Color b) => new(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
  public static Color operator -(Color a, Color b) => new(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
  public static Color operator *(Color a, Color b) => new(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);
  public static Color operator /(Color a, Color b) => new(a.R / b.R, a.G / b.G, a.B / b.B, a.A / b.A);
  public static Color operator *(Color a, float scalar) => new(a.R * scalar, a.G * scalar, a.B * scalar, a.A * scalar);
  public static Color operator /(Color a, float scalar) => new(a.R / scalar, a.G / scalar, a.B / scalar, a.A / scalar);

  public static explicit operator Color(Vector4 vector) => new(vector.X, vector.Y, vector.Z, vector.W);
  public static explicit operator Color(Vector3 vector) => new(vector.X, vector.Y, vector.Z);
  public static explicit operator Vector4(Color color) => new(color.R, color.G, color.B, color.A);
  public static explicit operator Vector3(Color color) => new(color.R, color.G, color.B);

  public static implicit operator Color(Color32 color)
  {
    var r = color.R / 255.0f;
    var g = color.G / 255.0f;
    var b = color.B / 255.0f;
    var a = color.A / 255.0f;

    return new Color(r, g, b, a);
  }
}

/// <summary>A 32-bit representation of color.</summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Color32(byte R, byte G, byte B, byte A = 255)
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

  public static Color32 Lerp(Color32 a, Color32 b, float t) => new(
    (byte) Maths.Lerp(a.R, b.R, t),
    (byte) Maths.Lerp(a.G, b.G, t),
    (byte) Maths.Lerp(a.B, b.B, t),
    (byte) Maths.Lerp(a.A, b.A, t)
  );

  public byte R = R;
  public byte G = G;
  public byte B = B;
  public byte A = A;

  public readonly Color32 Lighten(byte amount) => new((byte) (R + amount), (byte) (G + amount), (byte) (B + amount));
  public readonly Color32 Darken(byte amount) => new((byte) (R - amount), (byte) (G - amount), (byte) (B - amount));

  public override string ToString() => $"<{R}, {G}, {B}, {A}>";

  public static Color32 operator +(Color32 a, Color32 b) => new((byte) (a.R + b.R), (byte) (a.G + b.G), (byte) (a.B + b.B), (byte) (a.A + b.A));
  public static Color32 operator -(Color32 a, Color32 b) => new((byte) (a.R - b.R), (byte) (a.G - b.G), (byte) (a.B - b.B), (byte) (a.A - b.A));
  public static Color32 operator *(Color32 a, Color32 b) => new((byte) (a.R * b.R), (byte) (a.G * b.G), (byte) (a.B * b.B), (byte) (a.A * b.A));
  public static Color32 operator /(Color32 a, Color32 b) => new((byte) (a.R / b.R), (byte) (a.G / b.G), (byte) (a.B / b.B), (byte) (a.A / b.A));
  public static Color32 operator *(Color32 a, int scalar) => new((byte) (a.R * scalar), (byte) (a.G * scalar), (byte) (a.B * scalar), (byte) (a.A * scalar));
  public static Color32 operator /(Color32 a, int scalar) => new((byte) (a.R / scalar), (byte) (a.G / scalar), (byte) (a.B / scalar), (byte) (a.A / scalar));

  public static implicit operator Color32(Color color)
  {
    var r = (byte) (color.R * 255.0f);
    var g = (byte) (color.G * 255.0f);
    var b = (byte) (color.B * 255.0f);
    var a = (byte) (color.A * 255.0f);

    return new Color32(r, g, b, a);
  }
}
