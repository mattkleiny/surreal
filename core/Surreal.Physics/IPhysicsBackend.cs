using Surreal.Entities;
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
    public IPhysicsWorld2d CreatePhysicsWorld2d() => new NullPhysicsWorld2d();
    public IPhysicsWorld3d CreatePhysicsWorld3d() => new NullPhysicsWorld3d();

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

      public void SetBodyPosition(PhysicsHandle handle, Vector2 position)
      {
      }

      public float GetBodyRotation(PhysicsHandle handle)
      {
        return 0;
      }

      public void SetBodyRotation(PhysicsHandle handle, float rotation)
      {
      }

      public void DeleteBody(PhysicsHandle handle)
      {
      }

      public Vector2 GetBodySize(PhysicsHandle handle)
      {
        return Vector2.One;
      }

      public void SetBodySize(PhysicsHandle handle, Vector2 size)
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

      public void SetBodyPosition(PhysicsHandle handle, Vector3 position)
      {
      }

      public Quaternion GetBodyRotation(PhysicsHandle handle)
      {
        return Quaternion.Identity;
      }

      public void SetBodyRotation(PhysicsHandle handle, Quaternion rotation)
      {
      }

      public void DeleteBody(PhysicsHandle handle)
      {
      }

      public Vector3 GetBodySize(PhysicsHandle handle)
      {
        return Vector3.One;
      }

      public void SetBodySize(PhysicsHandle handle, Vector3 size)
      {
      }
    }
  }
}

