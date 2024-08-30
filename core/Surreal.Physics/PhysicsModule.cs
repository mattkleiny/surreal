using Surreal.Entities;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal.Physics;

public record struct FixedUpdateEvent(DeltaTime DeltaTime);

public record struct Collider : IComponent<Collider>
{
  public Vector3 Center { get; set; }
  public Vector3 Size { get; set; }
  public bool IsTrigger { get; set; }
}

public record struct Rigidbody(PhysicsHandle Handle) : IComponent<Rigidbody>
{
  public Vector3 Position { get; set; }
  public Vector3 Velocity { get; set; }
  public Vector3 AngularVelocity { get; set; }
  public float Mass { get; set; }
  public float Drag { get; set; }
}

public record struct Transform : IComponent<Transform>
{
  public Vector3 Position { get; set; }
  public Quaternion Rotation { get; set; }
  public Vector3 Scale { get; set; }
}

/// <summary>
/// A <see cref="IServiceModule"/> for the physics system.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PhysicsModule : IServiceModule
{
  /// <summary>
  /// The <see cref="IPhysicsBackend"/> to be used.
  /// </summary>
  public IPhysicsBackend Backend { get; } = new PhysicsBackend();

  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService(Backend);

    registry.AddSystem<FixedUpdateEvent>(ProcessCollisions);
    registry.AddSystem<FixedUpdateEvent>(ProcessRigidbodies);
    registry.AddSystem<FixedUpdateEvent>(SyncTransformsToRigidbodies);
  }

  private void ProcessCollisions(in FixedUpdateEvent @event, ref Collider collider)
  {
    throw new NotImplementedException();
  }

  private void ProcessRigidbodies(in FixedUpdateEvent @event, ref Rigidbody rigidbody, IPhysicsWorld3d world)
  {
    rigidbody.Position = world.GetBodyPosition(rigidbody.Handle);
    rigidbody.Velocity += world.Gravity * @event.DeltaTime;
  }

  private void SyncTransformsToRigidbodies(in FixedUpdateEvent @event, ref Transform transform, ref Rigidbody rigidbody)
  {
    transform.Position += rigidbody.Velocity * @event.DeltaTime;
    transform.Rotation *= Quaternion.Identity; // TODO: Implement rotation
  }
}
