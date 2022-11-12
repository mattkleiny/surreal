namespace Surreal.Mathematics;

/// <summary>A volume in 3-space.</summary>
public readonly record struct Volume(float Width, float Height, float Depth)
{
  public float Total => Width * Height * Depth;

  public override string ToString()
  {
    return $"{Width}x{Height}x{Depth} ({Total} units)";
  }

  public static Volume operator +(Volume a, float scalar)
  {
    return new Volume(a.Width + scalar, a.Height + scalar, a.Depth + scalar);
  }

  public static Volume operator -(Volume a, float scalar)
  {
    return new Volume(a.Width - scalar, a.Height - scalar, a.Depth - scalar);
  }

  public static Volume operator *(Volume a, float scalar)
  {
    return new Volume(a.Width * scalar, a.Height * scalar, a.Depth * scalar);
  }

  public static Volume operator /(Volume a, float scalar)
  {
    return new Volume(a.Width / scalar, a.Height / scalar, a.Depth / scalar);
  }

  public static Volume operator +(Volume a, Volume b)
  {
    return new Volume(a.Width + b.Width, a.Height + b.Height, a.Depth + b.Depth);
  }

  public static Volume operator -(Volume a, Volume b)
  {
    return new Volume(a.Width - b.Width, a.Height - b.Height, a.Depth - b.Depth);
  }

  public static Volume operator *(Volume a, Volume b)
  {
    return new Volume(a.Width * b.Width, a.Height * b.Height, a.Depth * b.Depth);
  }

  public static Volume operator /(Volume a, Volume b)
  {
    return new Volume(a.Width / b.Width, a.Height / b.Height, a.Depth / b.Depth);
  }
}



