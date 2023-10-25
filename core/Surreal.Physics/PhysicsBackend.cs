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
      var deltaTimeSqr = deltaTime.Seconds * deltaTime.Seconds;

      // apply gravity
      foreach (ref var body in _bodies)
      {
        body.Acceleration += Gravity;
      }

      // verlet integration
      foreach (ref var body in _bodies)
      {
        var velocity = body.CurrentPosition - body.PreviousPosition;

        body.PreviousPosition = body.CurrentPosition;
        body.CurrentPosition += velocity + body.Acceleration * deltaTimeSqr;

        body.Acceleration = Vector2.Zero;
      }
    }

    public PhysicsHandle CreateBody(Vector2 initialPosition)
    {
      var index = _bodies.Add(new PhysicsBody
      {
        CurrentPosition = initialPosition,
        PreviousPosition = initialPosition
      });

      return PhysicsHandle.FromArenaIndex(index);
    }

    public Vector2 GetBodyPosition(PhysicsHandle handle)
    {
      return _bodies[handle].CurrentPosition;
    }

    public void SetBodyPosition(PhysicsHandle handle, Vector2 position)
    {
      ref var body = ref _bodies[handle];

      body.PreviousPosition = position;
      body.CurrentPosition = position;
    }

    public Vector2 GetBodyVelocity(PhysicsHandle handle)
    {
      var body = _bodies[handle];

      return body.CurrentPosition - body.PreviousPosition;
    }

    public void AddBodyVelocity(PhysicsHandle handle, Vector2 velocity)
    {
      _bodies[handle].Acceleration += velocity;
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
      public Vector2 PreviousPosition;
      public Vector2 CurrentPosition;
      public Vector2 Acceleration;
    }
  }
}
