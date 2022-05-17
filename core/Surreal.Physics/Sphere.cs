using Surreal.Physics.Collisions;
using Surreal.Physics.Dynamics;

namespace Surreal.Physics;

/// <summary>A sphere in 3-space with support for collision detection and dynamics.</summary>
public sealed record Sphere : ICollisionObject, IDynamicObject
{
  public Vector3 Position { get; set; } = Vector3.Zero;
  public Vector3 Velocity { get; set; } = Vector3.Zero;
  public Vector3 Force    { get; set; } = Vector3.Zero;
  public float   Mass     { get; set; } = 1f;
  public float   Radius   { get; set; } = 1f;

  public bool CollidesWith(ICollisionObject other, out CollisionDetails details)
  {
    details = default;

    return other switch
    {
      Sphere sphere => CollisionDetection.CheckCollision(this, sphere, out details),
      Plane plane   => CollisionDetection.CheckCollision(this, plane, out details),
      _             => false
    };
  }
}
