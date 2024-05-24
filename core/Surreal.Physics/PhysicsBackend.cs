using Surreal.Collections;
using Surreal.Colors;
using Surreal.Diagnostics.Gizmos;
using Surreal.Timing;

namespace Surreal.Physics;

/// <summary>
/// A lightweight C#-based implementation of the physics backend.
/// </summary>
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
    private const float BodyRadius = 5f;
    private const float WorldRadius = 80f;

    private readonly Arena<VerletBody> _bodies = [];

    /// <summary>
    /// The number of iterations to perform per tick.
    /// </summary>
    public int Iterations { get; set; } = 1;

    /// <inheritdoc/>
    public Vector2 Gravity { get; set; } = new(0f, -100f);

    /// <inheritdoc/>
    public void Tick(DeltaTime deltaTime)
    {
      var stepTime = deltaTime / Iterations;

      for (int i = 0; i < Iterations; i++)
      {
        ApplyConstraints();
        ApplyCollisions();
        ApplyVerlet(stepTime);
      }
    }

    /// <inheritdoc/>
    public void Reset()
    {
      _bodies.Clear();
    }

    private void ApplyConstraints()
    {
      var position = Vector2.Zero;

      foreach (ref var body in _bodies)
      {
        var delta = body.CurrentPosition - position;
        var distance = delta.Length();

        if (distance > WorldRadius - BodyRadius)
        {
          var normal = delta / distance;

          body.CurrentPosition = position + normal * (distance - BodyRadius);
        }
      }
    }

    private unsafe void ApplyCollisions()
    {
      foreach (ref var a in _bodies)
      foreach (ref var b in _bodies)
      {
        if (Unsafe.AsPointer(ref a) == Unsafe.AsPointer(ref b))
          continue;

        var axis = a.CurrentPosition - b.CurrentPosition;
        var distance = axis.Length();

        if (distance < BodyRadius)
        {
          var normal = axis / distance;
          var delta = BodyRadius - distance;

          a.CurrentPosition += 0.5f * delta * normal;
          b.CurrentPosition -= 0.5f * delta * normal;
        }
      }
    }

    private void ApplyVerlet(float deltaTime)
    {
      foreach (ref var body in _bodies)
      {
        var velocity = body.CurrentPosition - body.PreviousPosition;

        body.PreviousPosition =  body.CurrentPosition;
        body.CurrentPosition  += velocity + Gravity * (deltaTime * deltaTime);
      }
    }

    public PhysicsHandle CreateBody(Vector2 initialPosition)
    {
      var index = _bodies.Add(new VerletBody
      {
        CurrentPosition  = initialPosition,
        PreviousPosition = initialPosition
      });

      return PhysicsHandle.FromArenaIndex(index);
    }

    public Vector2 GetBodyPosition(PhysicsHandle handle)
    {
      return _bodies[handle].CurrentPosition;
    }

    public void DeleteBody(PhysicsHandle handle)
    {
      _bodies.Remove(handle);
    }

    /// <summary>
    /// A simple verlet body for the 2d physics world.
    /// </summary>
    private struct VerletBody
    {
      public Vector2 PreviousPosition;
      public Vector2 CurrentPosition;
    }
  }
}
