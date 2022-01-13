namespace Surreal.Physics;

/// <summary>An object that can participate in dynamics simulations.</summary>
public interface IDynamicObject : ICollisionObject
{
  ref Vector3 Position { get; }
  ref Vector3 Velocity { get; }
  ref Vector3 Force    { get; }

  float Mass { get; }
}
