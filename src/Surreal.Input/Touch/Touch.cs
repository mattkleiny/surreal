using Surreal.Mathematics.Linear;

namespace Surreal.Input.Touch
{
  public readonly record struct Touch(uint Id, Point2 Position, float Pressure)
  {
    public override string ToString() => $"Touch at {Position.ToString()} with {Pressure.ToString("P")}";
  }
}
