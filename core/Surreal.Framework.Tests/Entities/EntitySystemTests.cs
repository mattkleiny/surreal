using Surreal.Physics;
using Surreal.Scenes;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal.Entities;

public class EntitySystemTests
{
  [Test]
  public void it_should_work()
  {
    var services = new ServiceRegistry();
    var world = new EntityWorld(services);
    var entity = world.SpawnEntity();

    world.AddComponent(entity, new Transform { Position = new(0f, 10f) });
    world.AddComponent(entity, new Rigidbody { Gravity = new(0f, -9.81f) });

    world.AddSystem<TickEvent>(SyncTransformToRigidbody);

    world.Execute(new TickEvent(DeltaTime.Default));
  }

  private static readonly EntityQuery Query = new EntityQuery()
    .Include<Transform>()
    .Include<Rigidbody>();

  private static void TestSystem(in TickEvent @event, EntityWorld world)
  {
    foreach (var entity in world.Query(Query))
    {
      ref var transform = ref world.GetComponent<Transform>(entity);
      ref var rigidbody = ref world.GetComponent<Rigidbody>(entity);

      rigidbody.Velocity += rigidbody.Gravity * @event.DeltaTime;
      transform.Position += rigidbody.Velocity * @event.DeltaTime;
    }
  }

  private static void SyncTransformToRigidbody(
    in TickEvent @event,
    ref Transform transform,
    ref Rigidbody rigidbody,
    IPhysicsWorld2d world)
  {
    rigidbody.Velocity += rigidbody.Gravity * @event.DeltaTime;
    transform.Position += rigidbody.Velocity * @event.DeltaTime;
  }

  private struct Transform : IComponent<Transform>
  {
    public Vector2 Position { get; set; }
  }

  private struct Rigidbody() : IComponent<Rigidbody>
  {
    public Vector2 Velocity { get; set; }
    public Vector2 Gravity { get; set; } = new(0f, -9.81f);
  }
}
