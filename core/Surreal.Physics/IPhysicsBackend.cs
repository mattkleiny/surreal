using Surreal.Timing;

namespace Surreal.Physics;

/// <summary>
/// An abstraction over the different types of physics backends available.
/// </summary>
public interface IPhysicsBackend
{
  /// <summary>
  /// Creates a new 2d physics world.
  /// </summary>
  IPhysicsWorld2d CreatePhysicsWorld2d();
}

/// <summary>
/// Represents a physics world that can be updated via the physics system.
/// </summary>
public interface IPhysicsWorld
{
  /// <summary>
  /// Updates the physics world.
  /// </summary>
  void Tick(DeltaTime deltaTime);
}

/// <summary>
/// Represents a 2d world that can be updated via the physics system.
/// </summary>
public interface IPhysicsWorld2d : IPhysicsWorld
{
  // top-level properties
  Vector2 Gravity { get; set; }

  // bodies
  PhysicsHandle CreateBody();
  Vector2 GetBodyPosition(PhysicsHandle handle);
  void SetBodyPosition(PhysicsHandle handle, Vector2 position);
  float GetBodyRotation(PhysicsHandle handle);
  void SetBodyRotation(PhysicsHandle handle, float rotation);
  Vector2 GetBodyScale(PhysicsHandle handle);
  void SetBodyScale(PhysicsHandle handle, Vector2 scale);
  void GetBodyTransform(PhysicsHandle handle, out Vector2 position, out float rotation, out Vector2 scale);
  void SetBodyTransform(PhysicsHandle handle, Vector2 position, float rotation, Vector2 scale);
  Vector2 GetBodyVelocity(PhysicsHandle handle);
  void SetBodyVelocity(PhysicsHandle handle, Vector2 velocity);
  float GetBodyTorque(PhysicsHandle handle);
  void SetBodyTorque(PhysicsHandle handle, float torque);
  void DeleteBody(PhysicsHandle handle);
}
