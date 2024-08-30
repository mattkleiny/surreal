using Surreal.Collections;
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

  public IPhysicsWorld3d CreatePhysicsWorld3d()
  {
    return new PhysicsWorld3d();
  }

  /// <summary>
  /// The <see cref="IPhysicsWorld2d"/> implementation for this backend.
  /// </summary>
  private sealed class PhysicsWorld2d : IPhysicsWorld2d
  {
    private const float BodyRadius = 5f;
    private const float WorldRadius = 80f;

    private readonly Arena<Body> _bodies = [];

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

      foreach (var body in _bodies)
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

    private void ApplyCollisions()
    {
      foreach (var a in _bodies)
      foreach (var b in _bodies)
      {
        if (a == b)
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
      foreach (var body in _bodies)
      {
        var velocity = body.CurrentPosition - body.PreviousPosition;

        body.PreviousPosition = body.CurrentPosition;
        body.CurrentPosition += velocity + Gravity * (deltaTime * deltaTime);
      }
    }

    public PhysicsHandle CreateBody(Vector2 initialPosition)
    {
      var index = _bodies.Add(new Body
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
      _bodies[handle].CurrentPosition = position;
    }

    public float GetBodyRotation(PhysicsHandle handle)
    {
      return _bodies[handle].CurrentRotation;
    }

    public void SetBodyRotation(PhysicsHandle handle, float rotation)
    {
      _bodies[handle].CurrentRotation = rotation;
    }

    public void DeleteBody(PhysicsHandle handle)
    {
      _bodies.Remove(handle);
    }

    private sealed class Body
    {
      public Vector2 PreviousPosition;
      public Vector2 CurrentPosition;
      public float PreviousRotation;
      public float CurrentRotation;
    }
  }

  /// <summary>
  /// The <see cref="IPhysicsWorld3d"/> implementation for this backend.
  /// </summary>
  private sealed class PhysicsWorld3d : IPhysicsWorld3d
  {
    private readonly Arena<Body> _bodies = [];

    public Vector3 Gravity { get; set; }

    /// <inheritdoc/>
    public void Tick(DeltaTime deltaTime)
    {
    }

    /// <inheritdoc/>
    public void Reset()
    {
      _bodies.Clear();
    }

    public PhysicsHandle CreateBody(Vector3 initialPosition)
    {
      var index = _bodies.Add(new Body
      {
        Position = initialPosition
      });

      return PhysicsHandle.FromArenaIndex(index);
    }

    public Vector3 GetBodyPosition(PhysicsHandle handle)
    {
      return _bodies[handle].Position;
    }

    public void SetBodyPosition(PhysicsHandle handle, Vector3 position)
    {
      _bodies[handle].Position = position;
    }

    public Quaternion GetBodyRotation(PhysicsHandle handle)
    {
      return _bodies[handle].Rotation;
    }

    public void SetBodyRotation(PhysicsHandle handle, Quaternion rotation)
    {
      _bodies[handle].Rotation = rotation;
    }

    public void DeleteBody(PhysicsHandle handle)
    {
      _bodies.Remove(handle);
    }

    private sealed class Body
    {
      public Vector3 Position;
      public Quaternion Rotation;
    }
  }
}
