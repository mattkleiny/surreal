using System;

namespace Surreal.Mathematics.Linear
{
  public readonly struct Volume : IEquatable<Volume>
  {
    public Volume(int width, int height, int depth)
    {
      Width  = width;
      Height = height;
      Depth  = depth;
    }

    public readonly int Width;
    public readonly int Height;
    public readonly int Depth;

    public int Total => Width * Height * Depth;

    public override string ToString() => $"{Width.ToString()}x{Height.ToString()}x{Depth.ToString()} ({Total.ToString()} units)";

    public          bool Equals(Volume other) => Width == other.Width && Height == other.Height && Depth == other.Depth;
    public override bool Equals(object? obj)  => obj is Volume other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Width, Height, Depth);

    public static bool operator ==(Volume left, Volume right) => left.Equals(right);
    public static bool operator !=(Volume left, Volume right) => !left.Equals(right);

    public static Volume operator +(Volume a, int scalar) => new(a.Width + scalar, a.Height + scalar, a.Depth + scalar);
    public static Volume operator -(Volume a, int scalar) => new(a.Width - scalar, a.Height - scalar, a.Depth - scalar);
    public static Volume operator *(Volume a, int scalar) => new(a.Width * scalar, a.Height * scalar, a.Depth * scalar);
    public static Volume operator /(Volume a, int scalar) => new(a.Width / scalar, a.Height / scalar, a.Depth / scalar);

    public static Volume operator +(Volume a, Volume b) => new(a.Width + b.Width, a.Height + b.Height, a.Depth + b.Depth);
    public static Volume operator -(Volume a, Volume b) => new(a.Width - b.Width, a.Height - b.Height, a.Depth - b.Depth);
    public static Volume operator *(Volume a, Volume b) => new(a.Width * b.Width, a.Height * b.Height, a.Depth * b.Depth);
    public static Volume operator /(Volume a, Volume b) => new(a.Width / b.Width, a.Height / b.Height, a.Depth / b.Depth);
  }
}