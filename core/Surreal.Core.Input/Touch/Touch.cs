using Surreal.Mathematics.Linear;

namespace Surreal.Input.Touch {
  public readonly struct Touch {
    public readonly uint     Id;
    public readonly Vector2I Position;
    public readonly float    Pressure;

    public Touch(uint id, Vector2I position, float pressure) {
      Id       = id;
      Position = position;
      Pressure = pressure;
    }

    public override string ToString() => $"Touch at {Position} with {Pressure:P}";
  }
}