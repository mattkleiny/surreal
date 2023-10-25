using Surreal.Timing;

namespace Surreal.Physics;

/// <summary>
/// An abstraction over the different types of physics backends available.
/// </summary>
public interface IPhysicsBackend
{
  IPhysicsWorld2d CreatePhysicsWorld2d();
  IPhysicsWorld2d CreatePhysicsWorld3d();
}

/// <summary>
/// A physics world that contains objects and simulates physics.
/// </summary>
public interface IPhysicsWorld
{
  /// <summary>
  /// Updates the physics world.
  /// </summary>
  void Tick(DeltaTime deltaTime);
}

/// <summary>
/// A 2d world in a <see cref="IPhysicsBackend"/>.
/// </summary>
public interface IPhysicsWorld2d : IPhysicsWorld
{
  // top-level properties
  Vector2 Gravity { get; set; }

  // bodies
  PhysicsHandle CreateBody(Vector2 initialPosition);
  Vector2 GetBodyPosition(PhysicsHandle handle);
  void SetBodyPosition(PhysicsHandle handle, Vector2 position);
  Vector2 GetBodyVelocity(PhysicsHandle handle);
  void AddBodyVelocity(PhysicsHandle handle, Vector2 velocity);
  void DeleteBody(PhysicsHandle handle);
}
