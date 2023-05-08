namespace Surreal.Mathematics;

/// <summary>
/// An area in 2-space.
/// </summary>
public readonly record struct Area(float Width, float Height)
{
  public float Total => Width * Height;

  public override string ToString() => $"{Width}x{Height} ({Total} units)";

  public static Area operator +(Area a, float scalar) => new(a.Width + scalar, a.Height + scalar);
  public static Area operator -(Area a, float scalar) => new(a.Width - scalar, a.Height - scalar);
  public static Area operator *(Area a, float scalar) => new(a.Width * scalar, a.Height * scalar);
  public static Area operator /(Area a, float scalar) => new(a.Width / scalar, a.Height / scalar);

  public static Area operator +(Area a, Area b) => new(a.Width + b.Width, a.Height + b.Height);
  public static Area operator -(Area a, Area b) => new(a.Width - b.Width, a.Height - b.Height);
  public static Area operator *(Area a, Area b) => new(a.Width * b.Width, a.Height * b.Height);
  public static Area operator /(Area a, Area b) => new(a.Width / b.Width, a.Height / b.Height);
}
