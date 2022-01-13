namespace Surreal.Physics.Collisions;

/// <summary>An object that can participate in collision detections.</summary>
public interface ICollisionObject
{
  bool CollidesWith(ICollisionObject other, out CollisionDetails details);
}

/// <summary>Details information about a collision between two objects.</summary>
public readonly record struct CollisionDetails(ICollisionObject Object1, ICollisionObject Object2)
{
  public Vector3 Position { get; init; } = Vector3.Zero;
  public Vector3 Normal   { get; init; } = Vector3.Zero;
}

/// <summary>Commonly used collision detection routines.</summary>
internal static class CollisionDetection
{
  public static bool CheckCollision(Sphere sphere1, Sphere sphere2, out CollisionDetails details)
  {
    throw new NotImplementedException();
  }

  public static bool CheckCollision(Sphere sphere, Plane plane, out CollisionDetails details)
  {
    throw new NotImplementedException();
  }
}
