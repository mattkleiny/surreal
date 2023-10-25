using Surreal.Collections;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal.Physics;

/// <summary>
/// A lightweight C#-based implementation of the physics backend.
/// </summary>
[RegisterService(typeof(IPhysicsBackend))]
internal sealed class PhysicsBackend : IPhysicsBackend
{
  public IPhysicsWorld2d CreatePhysicsWorld2d()
  {
    return new PhysicsWorld2d();
  }

  public IPhysicsWorld2d CreatePhysicsWorld3d()
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// The <see cref="IPhysicsWorld2d"/> implementation for this backend.
  /// </summary>
  private sealed class PhysicsWorld2d : IPhysicsWorld2d
  {
    private readonly Arena<PhysicsBody> _bodies = new();

    public Vector2 Gravity { get; set; } = new(0f, -9.81f);

    public void Tick(DeltaTime deltaTime)
    {
      foreach (ref var body in _bodies)
      {
        body.Position += body.Velocity * deltaTime;
        body.Rotation += body.Torque * deltaTime;

        body.Velocity += Gravity * deltaTime;
      }
    }

    public PhysicsHandle CreateBody()
    {
      return PhysicsHandle.FromArenaIndex(_bodies.Add(new PhysicsBody()));
    }

    public Vector2 GetBodyPosition(PhysicsHandle handle)
    {
      return _bodies[handle].Position;
    }

    public void SetBodyPosition(PhysicsHandle handle, Vector2 position)
    {
      _bodies[handle].Position = position;
    }

    public float GetBodyRotation(PhysicsHandle handle)
    {
      return _bodies[handle].Rotation;
    }

    public void SetBodyRotation(PhysicsHandle handle, float rotation)
    {
      _bodies[handle].Rotation = rotation;
    }

    public Vector2 GetBodyScale(PhysicsHandle handle)
    {
      return _bodies[handle].Scale;
    }

    public void SetBodyScale(PhysicsHandle handle, Vector2 scale)
    {
      _bodies[handle].Scale = scale;
    }

    public void GetBodyTransform(PhysicsHandle handle, out Vector2 position, out float rotation, out Vector2 scale)
    {
      var body = _bodies[handle];

      position = body.Position;
      rotation = body.Rotation;
      scale = body.Scale;
    }

    public void SetBodyTransform(PhysicsHandle handle, Vector2 position, float rotation, Vector2 scale)
    {
      ref var body = ref _bodies[handle];

      body.Position = position;
      body.Rotation = rotation;
      body.Scale = scale;
    }

    public Vector2 GetBodyVelocity(PhysicsHandle handle)
    {
      return _bodies[handle].Velocity;
    }

    public void SetBodyVelocity(PhysicsHandle handle, Vector2 velocity)
    {
      _bodies[handle].Velocity = new Vector2(velocity.X, velocity.Y);
    }

    public float GetBodyTorque(PhysicsHandle handle)
    {
      return _bodies[handle].Torque;
    }

    public void SetBodyTorque(PhysicsHandle handle, float torque)
    {
      _bodies[handle].Torque = torque;
    }

    public void DeleteBody(PhysicsHandle handle)
    {
      _bodies.Remove(handle);
    }

    /// <summary>
    /// A single physics body.
    /// </summary>
    private struct PhysicsBody
    {
      public Vector2 Position;
      public float Rotation;
      public Vector2 Scale;
      public Vector2 Velocity;
      public float Torque;
    }
  }
}
