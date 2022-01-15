namespace Surreal.Mathematics;

#pragma warning disable S1104
#pragma warning disable S2328

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

  public float R;
  public float G;
  public float B;
  public float A;

  public Color Lighten(float amount) => new(R + amount, G + amount, B + amount);
  public Color Darken(float amount)  => new(R - amount, G - amount, B - amount);

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
  public static explicit operator Color(Vector3 vector) => new(vector.X, vector.Y, vector.Z);
  public static explicit operator Vector4(Color color)  => new(color.R, color.G, color.B, color.A);
  public static explicit operator Vector3(Color color)  => new(color.R, color.G, color.B);

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
public struct Color32 : IEquatable<Color32>
{
  public static Color32 Parse(string raw)
  {
    throw new NotImplementedException();
  }

  public Color32(byte red, byte green, byte blue, byte alpha = 255)
  {
    R = red;
    G = green;
    B = blue;
    A = alpha;
  }

  public byte R;
  public byte G;
  public byte B;
  public byte A;

  public Color32 Lighten(byte amount) => new((byte) (R + amount), (byte) (G + amount), (byte) (B + amount));
  public Color32 Darken(byte amount)  => new((byte) (R - amount), (byte) (G - amount), (byte) (B - amount));

  public override string ToString() => $"<{R}, {G}, {B}, {A}>";

  public bool Equals(Color32 other)
  {
    return R == other.R &&
           G == other.G &&
           B == other.B &&
           A == other.A;
  }

  public override bool Equals(object? obj) => obj is Color32 other && Equals(other);

  public override int GetHashCode() => HashCode.Combine(R, G, B, A);

  public static bool operator ==(Color32 left, Color32 right) => left.Equals(right);
  public static bool operator !=(Color32 left, Color32 right) => !left.Equals(right);

  public static Color32 operator +(Color32 a, Color32 b)  => new((byte) (a.R + b.R), (byte) (a.G + b.G), (byte) (a.B + b.B), (byte) (a.A + b.A));
  public static Color32 operator -(Color32 a, Color32 b)  => new((byte) (a.R - b.R), (byte) (a.G - b.G), (byte) (a.B - b.B), (byte) (a.A - b.A));
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
