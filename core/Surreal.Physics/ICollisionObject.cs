namespace Surreal.Physics;

/// <summary>Details information about a collision between two objects.</summary>
public readonly record struct CollisionDetails(ICollisionObject Object1, ICollisionObject Object2)
{
  public Vector3 Position { get; init; } = Vector3.Zero;
  public Vector3 Normal   { get; init; } = Vector3.Zero;
}

/// <summary>An object that can participate in collision detections.</summary>
public interface ICollisionObject
{
  bool CollidesWith(ICollisionObject other, out CollisionDetails details);
}
