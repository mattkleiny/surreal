using Surreal.Mathematics.Linear;

namespace Surreal.Input.Touch
{
  public readonly struct Touch
  {
    public readonly uint   Id;
    public readonly Point2 Position;
    public readonly float  Pressure;

    public Touch(uint id, Point2 position, float pressure)
    {
      Id       = id;
      Position = position;
      Pressure = pressure;
    }

    public override string ToString() => $"Touch at {Position.ToString()} with {Pressure.ToString("P")}";
  }
}