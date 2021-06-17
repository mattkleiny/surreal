using System;
using System.Runtime.InteropServices;
using Surreal.Mathematics;

namespace Surreal.Graphics {
  [StructLayout(LayoutKind.Sequential)]
  public struct Color : IEquatable<Color> {
    public static readonly Color Black   = new(0, 0, 0);
    public static readonly Color Red     = new(255, 0, 0);
    public static readonly Color Green   = new(0, 255, 0);
    public static readonly Color Blue    = new(0, 0, 255);
    public static readonly Color Yellow  = new(255, 255, 0);
    public static readonly Color Magenta = new(255, 0, 255);
    public static readonly Color Grey    = new(205, 205, 205);
    public static readonly Color Brown   = new(168, 42, 42);
    public static readonly Color White   = new(255, 255, 255);
    public static readonly Color Clear   = new(0, 0, 0, 0);

    public static Color Lerp(Color a, Color b, float t) => new(
        Maths.Lerp(a.R, b.R, t),
        Maths.Lerp(a.G, b.G, t),
        Maths.Lerp(a.B, b.B, t),
        Maths.Lerp(a.A, b.A, t)
    );

    public Color(float red, float green, float blue, float alpha = 1f) {
      R = red;
      G = green;
      B = blue;
      A = alpha;
    }

    public float R;
    public float G;
    public float B;
    public float A;

    public override string ToString() => $"RGBA({R.ToString()}, {G.ToString()}, {B.ToString()}, {A.ToString()})";

    public bool Equals(Color other) {
      return Math.Abs(R - other.R) < float.Epsilon &&
             Math.Abs(G - other.G) < float.Epsilon &&
             Math.Abs(B - other.B) < float.Epsilon &&
             Math.Abs(A - other.A) < float.Epsilon;
    }

    public override bool Equals(object? obj) => obj is Color other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(R, G, B, A);

    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    public static Color operator +(Color a, Color b) => new(
        (byte) (a.R + b.R),
        (byte) (a.G + b.G),
        (byte) (a.B + b.B),
        (byte) (a.A + b.A)
    );

    public static Color operator -(Color a, Color b) => new(
        (byte) (a.R - b.R),
        (byte) (a.G - b.G),
        (byte) (a.B - b.B),
        (byte) (a.A - b.A)
    );
  }

  public static class ColorExtensions {
    public static Color NextColor(this Random random) => new(random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat());
  }
}