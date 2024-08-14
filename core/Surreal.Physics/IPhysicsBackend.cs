using Surreal.Timing;

namespace Surreal.Physics;

/// <summary>
/// An abstraction over the different types of physics backends available.
/// </summary>
public interface IPhysicsBackend
{
  static IPhysicsBackend Null { get; } = new NullPhysicsBackend();

  /// <summary>
  /// Creates a new <see cref="IPhysicsWorld3d"/>.
  /// </summary>
  IPhysicsWorld2d CreatePhysicsWorld2d();

  /// <summary>
  /// Creates a new <see cref="IPhysicsWorld3d"/>.
  /// </summary>
  IPhysicsWorld3d CreatePhysicsWorld3d();

  /// <summary>
  /// A no-op implementation of <see cref="IPhysicsBackend"/>.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullPhysicsBackend : IPhysicsBackend
  {
    public IPhysicsWorld2d CreatePhysicsWorld2d()
    {
      return new NullPhysicsWorld2d();
    }

    public IPhysicsWorld3d CreatePhysicsWorld3d()
    {
      return new NullPhysicsWorld3d();
    }

    /// <summary>
    /// A no-op implementation of <see cref="IPhysicsWorld2d"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    private sealed class NullPhysicsWorld2d : IPhysicsWorld2d
    {
      public Vector2 Gravity { get; set; }

      public void Tick(DeltaTime deltaTime)
      {
      }

      public void Reset()
      {
      }

      public PhysicsHandle CreateBody(Vector2 initialPosition)
      {
        return PhysicsHandle.None;
      }

      public Vector2 GetBodyPosition(PhysicsHandle handle)
      {
        return Vector2.Zero;
      }

      public void DeleteBody(PhysicsHandle handle)
      {
      }
    }

    /// <summary>
    /// A no-op implementation of <see cref="IPhysicsWorld3d"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    private sealed class NullPhysicsWorld3d : IPhysicsWorld3d
    {
      public Vector3 Gravity { get; set; }

      public void Tick(DeltaTime deltaTime)
      {
      }

      public void Reset()
      {
      }

      public PhysicsHandle CreateBody(Vector3 initialPosition)
      {
        return PhysicsHandle.None;
      }

      public Vector3 GetBodyPosition(PhysicsHandle handle)
      {
        return Vector3.Zero;
      }

      public void DeleteBody(PhysicsHandle handle)
      {
      }
    }
  }
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
  void DeleteBody(PhysicsHandle handle);
}

/// <summary>
/// A 3d world in a <see cref="IPhysicsBackend"/>.
/// </summary>
public interface IPhysicsWorld3d : IPhysicsWorld
{
  // top-level properties
  Vector3 Gravity { get; set; }

  // bodies
  PhysicsHandle CreateBody(Vector3 initialPosition);
  Vector3 GetBodyPosition(PhysicsHandle handle);
  void DeleteBody(PhysicsHandle handle);
}
