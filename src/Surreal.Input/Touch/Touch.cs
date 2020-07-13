using Surreal.Mathematics.Linear;

namespace Surreal.Input.Touch {
  public readonly struct Touch {
    public Touch(Vector2I position, float pressure) {
      Position = position;
      Pressure = pressure;
    }

    public readonly Vector2I Position;
    public readonly float    Pressure;

    public override string ToString() => $"Touch at {Position} with {Pressure:P}";
  }
}