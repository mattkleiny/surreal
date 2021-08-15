namespace Surreal.Mathematics.Linear
{
  public readonly record struct Area(float Width, float Height)
  {
    public float Total => Width * Height;

    public override string ToString() => $"{Width.ToString()}x{Height.ToString()} ({Total.ToString()} units)";
  }
}
