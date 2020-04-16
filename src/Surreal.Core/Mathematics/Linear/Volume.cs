namespace Surreal.Mathematics.Linear
{
  public readonly struct Volume
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

    // scalar operators

    public static Volume operator +(Volume a, int scalar) => new Volume(a.Width + scalar, a.Height + scalar, a.Depth + scalar);
    public static Volume operator -(Volume a, int scalar) => new Volume(a.Width - scalar, a.Height - scalar, a.Depth - scalar);
    public static Volume operator *(Volume a, int scalar) => new Volume(a.Width * scalar, a.Height * scalar, a.Depth * scalar);
    public static Volume operator /(Volume a, int scalar) => new Volume(a.Width / scalar, a.Height / scalar, a.Depth / scalar);

    // pair-wise operators

    public static Volume operator +(Volume a, Volume b) => new Volume(a.Width + b.Width, a.Height + b.Height, a.Depth + b.Depth);
    public static Volume operator -(Volume a, Volume b) => new Volume(a.Width - b.Width, a.Height - b.Height, a.Depth - b.Depth);
    public static Volume operator *(Volume a, Volume b) => new Volume(a.Width * b.Width, a.Height * b.Height, a.Depth * b.Depth);
    public static Volume operator /(Volume a, Volume b) => new Volume(a.Width / b.Width, a.Height / b.Height, a.Depth / b.Depth);

    public override string ToString() => $"{Width}x{Height}x{Depth} ({Total} units)";
  }
}
