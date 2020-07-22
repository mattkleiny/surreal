using System;
using System.Runtime.InteropServices;
using Surreal.Mathematics;

namespace Surreal.Graphics {
  [StructLayout(LayoutKind.Sequential)]
  public struct Color : IEquatable<Color> {
    public static readonly Color Black   = new Color(0, 0, 0);
    public static readonly Color Red     = new Color(255, 0, 0);
    public static readonly Color Green   = new Color(0, 255, 0);
    public static readonly Color Blue    = new Color(0, 0, 255);
    public static readonly Color Yellow  = new Color(255, 255, 0);
    public static readonly Color Magenta = new Color(255, 0, 255);
    public static readonly Color Grey    = new Color(205, 205, 205);
    public static readonly Color Brown   = new Color(168, 42, 42);
    public static readonly Color White   = new Color(255, 255, 255);
    public static readonly Color Clear   = new Color(0, 0, 0, 0);

    public static Color FromPackedRGB(uint packed) => new Color(
        red: (byte) (packed >> 16 & 0xFF),
        green: (byte) (packed >> 8 & 0xFF),
        blue: (byte) (packed >> 0 & 0xFF)
    );

    public static Color FromPackedRGBA(uint packed) => new Color(
        red: (byte) (packed >> 24 & 0xFF),
        green: (byte) (packed >> 16 & 0xFF),
        blue: (byte) (packed >> 8 & 0xFF),
        alpha: (byte) (packed >> 0 & 0xFF)
    );

    public static Color Lerp(Color a, Color b, float t) => new Color(
        (byte) Maths.Lerp(a.R, b.R, t),
        (byte) Maths.Lerp(a.G, b.G, t),
        (byte) Maths.Lerp(a.B, b.B, t),
        (byte) Maths.Lerp(a.A, b.A, t)
    );

    public Color(byte red, byte green, byte blue, byte alpha = 255) {
      R = red;
      G = green;
      B = blue;
      A = alpha;
    }

    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public uint RGB  => Pack(R, G, B);
    public uint RGBA => Pack(R, G, B, A);
    public uint BGR  => Pack(B, G, R);
    public uint BGRA => Pack(B, G, R, A);

    public override string ToString() => $"RGBA({R}, {G}, {B}, {A})";

    public bool Equals(Color other) {
      return R == other.R &&
             G == other.G &&
             B == other.B &&
             A == other.A;
    }

    public override bool Equals(object? obj) {
      return obj is Color other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(R, G, B, A);

    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    public static Color operator +(Color a, Color b) => new Color(
        (byte) (a.R + b.R),
        (byte) (a.G + b.G),
        (byte) (a.B + b.B),
        (byte) (a.A + b.A)
    );

    public static Color operator -(Color a, Color b) => new Color(
        (byte) (a.R - b.R),
        (byte) (a.G - b.G),
        (byte) (a.B - b.B),
        (byte) (a.A - b.A)
    );

    private static uint Pack(byte first, byte second, byte third) {
      return (uint) ((first << 24) | (second << 16) | (third << 8));
    }

    private static uint Pack(byte first, byte second, byte third, byte forth) {
      return (uint) ((first << 24) | (second << 16) | (third << 8) | (forth << 0));
    }
  }

  public static class ColorExtensions {
    public static Color NextColor(this Random random) => new Color(
        (byte) random.Next(0, 255),
        (byte) random.Next(0, 255),
        (byte) random.Next(0, 255),
        (byte) random.Next(0, 255)
    );
  }
}