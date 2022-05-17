namespace Surreal.Physics.Dynamics;

/// <summary>An object that can participate in dynamics simulations.</summary>
public interface IDynamicObject
{
  Vector3 Position { get; set; }
  Vector3 Velocity { get; set; }
  Vector3 Force    { get; set; }

  float Mass { get; }
}
