namespace Surreal.Mathematics.Linear;

/// <summary>A volume in 3-space.</summary>
public readonly record struct Volume(int Width, int Height, int Depth)
{
  public int Total => Width * Height * Depth;

  public override string ToString() => $"{Width.ToString()}x{Height.ToString()}x{Depth.ToString()} ({Total.ToString()} units)";

  public static Volume operator +(Volume a, int scalar) => new(a.Width + scalar, a.Height + scalar, a.Depth + scalar);
  public static Volume operator -(Volume a, int scalar) => new(a.Width - scalar, a.Height - scalar, a.Depth - scalar);
  public static Volume operator *(Volume a, int scalar) => new(a.Width * scalar, a.Height * scalar, a.Depth * scalar);
  public static Volume operator /(Volume a, int scalar) => new(a.Width / scalar, a.Height / scalar, a.Depth / scalar);

  public static Volume operator +(Volume a, Volume b) => new(a.Width + b.Width, a.Height + b.Height, a.Depth + b.Depth);
  public static Volume operator -(Volume a, Volume b) => new(a.Width - b.Width, a.Height - b.Height, a.Depth - b.Depth);
  public static Volume operator *(Volume a, Volume b) => new(a.Width * b.Width, a.Height * b.Height, a.Depth * b.Depth);
  public static Volume operator /(Volume a, Volume b) => new(a.Width / b.Width, a.Height / b.Height, a.Depth / b.Depth);
}