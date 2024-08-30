using Surreal.Entities;
using Surreal.Services;

namespace Surreal.Physics;

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

    registry.AddSystem<FixedTickEvent>(UpdatePhysicsWorld);
    registry.AddSystem<After<FixedTickEvent>>(SyncTransformsToRigidbodies);
  }

  /// <summary>
  /// A system which updates the physics world.
  /// </summary>
  private void UpdatePhysicsWorld(in FixedTickEvent @event, IPhysicsWorld2d world)
  {
    world.Tick(@event.DeltaTime);
  }

  /// <summary>
  /// A system which synchronizes transforms to their associated rigidbodies.
  /// </summary>
  private void SyncTransformsToRigidbodies(in After<FixedTickEvent> @event, ref Transform transform, Rigidbody rigidbody, IPhysicsWorld2d world)
  {
    transform.Position = world.GetBodyPosition(rigidbody.Handle);
    transform.Rotation = world.GetBodyRotation(rigidbody.Handle);
  }
}
