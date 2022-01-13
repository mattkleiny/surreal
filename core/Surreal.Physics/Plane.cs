using Surreal.Physics.Collisions;

namespace Surreal.Physics;

/// <summary>A plane in 3-space with support for collision detection.</summary>
public sealed record Plane : ICollisionObject
{
  public Vector3 Normal   { get; set; }
  public float   Distance { get; set; }

  public bool CollidesWith(ICollisionObject other, out CollisionDetails details)
  {
    details = default;

    return other switch
    {
      Sphere sphere => CollisionDetection.CheckCollision(sphere, this, out details),
      _             => false
    };
  }
}
