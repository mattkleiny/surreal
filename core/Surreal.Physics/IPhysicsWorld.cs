using Surreal.Timing;

namespace Surreal.Physics;

/// <summary>
/// A physics world that contains objects and simulates physics.
/// </summary>
public interface IPhysicsWorld
{
  /// <summary>
  /// Updates the physics world.
  /// </summary>
  void Tick(DeltaTime deltaTime);

  /// <summary>
  /// Clears all objects from the physics world.
  /// </summary>
  void Reset();
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
  void DeleteBody(PhysicsHandle handle);
}

/// <summary>
/// A 3d <see cref="IPhysicsWorld"/>.
/// </summary>
public interface IPhysicsWorld3d : IPhysicsWorld
{
  // top-level properties
  Vector3 Gravity { get; set; }

  // bodies
  PhysicsHandle CreateBody(Vector3 initialPosition);
  Vector3 GetBodyPosition(PhysicsHandle handle);
  void SetBodyPosition(PhysicsHandle handle, Vector3 position);
  void DeleteBody(PhysicsHandle handle);
}
