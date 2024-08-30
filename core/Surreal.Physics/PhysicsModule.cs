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

    registry.AddSystem<FixedTick>(OnPhysicsTick);
    registry.AddSystem<After<FixedTick>>(SyncTransforms);
    registry.AddSystem<Added<Rigidbody>>(OnRigidbodyAdded);
    registry.AddSystem<Removed<Rigidbody>>(OnRigidbodyRemoved);
  }

  private void OnPhysicsTick(in FixedTick @event, IPhysicsWorld2d world)
  {
    world.Tick(@event.DeltaTime);
  }

  private void SyncTransforms(in After<FixedTick> @event, ref Transform transform, Rigidbody rigidbody, IPhysicsWorld2d world)
  {
    transform.Position = world.GetBodyPosition(rigidbody.Handle);
    transform.Rotation = world.GetBodyRotation(rigidbody.Handle);
  }

  private void OnRigidbodyAdded(in Added<Rigidbody> @event, IPhysicsWorld2d world)
  {
  }

  private void OnRigidbodyRemoved(in Removed<Rigidbody> @event, IPhysicsWorld2d world)
  {
  }
}
