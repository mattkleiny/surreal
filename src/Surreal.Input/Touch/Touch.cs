using Surreal.Mathematics.Linear;

namespace Surreal.Input.Touch;

/// <summary>Represents a single touch on a <see cref="ITouchDevice"/>.</summary>
public readonly record struct Touch(uint Id, Point2 Position, float Pressure)
{
  public override string ToString() => $"Touch at {Position.ToString()} with {Pressure.ToString("P")}";
}