using Surreal.Scenes;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal.Entities;

public class EntityWorldTests
{
  [Test]
  public void it_should_spawn_an_entity()
  {
    var services = new ServiceRegistry();
    var world = new EntityWorld(services);
    var entity = world.SpawnEntity();

    Assert.That(world.HasEntity(entity), Is.True);
  }

  [Test]
  public void it_should_despawn_an_entity()
  {
    var services = new ServiceRegistry();
    var world = new EntityWorld(services);
    var entity = world.SpawnEntity();

    world.DespawnEntity(entity);

    Assert.That(world.HasEntity(entity), Is.False);
  }

  [Test]
  public void it_should_support_injection_of_manual_methods_with_optional_services()
  {
    var services = new ServiceRegistry();

    services.AddService<IPhysicsService>(new PhysicsService());

    var world = new EntityWorld(services);
    var entity = world.SpawnEntity();

    world.AddComponent(entity, new Transform());
    world.AddComponent(entity, new Rigidbody());

    world.AddSystem<TickEvent>(SyncTransformManual);

    world.Execute(new TickEvent(DeltaTime.Default));
  }

  [Test]
  public void it_should_support_injection_of_automatic_methods_with_optional_services()
  {
    var services = new ServiceRegistry();

    services.AddService<IPhysicsService>(new PhysicsService());

    var world = new EntityWorld(services);
    var entity = world.SpawnEntity();

    world.AddComponent(entity, new Transform());
    world.AddComponent(entity, new Rigidbody());

    world.AddSystem<TickEvent>(SyncTransformAutomatic);

    world.Execute(new TickEvent(DeltaTime.Default));
  }

  [Test]
  public void it_should_match_entities_based_on_inclusion_mask()
  {
    var services = new ServiceRegistry();
    var world = new EntityWorld(services);

    var entity = world.SpawnEntity();

    world.AddComponent(entity, new Transform());

    Assert.That(world.Query(new EntityQuery().Include<Transform>()), Has.Member(entity));
    Assert.That(world.Query(new EntityQuery().Include<Rigidbody>()), Is.Empty);
  }

  [Test]
  public void it_should_match_entities_based_on_exclusion_mask()
  {
    var services = new ServiceRegistry();
    var world = new EntityWorld(services);

    var entity = world.SpawnEntity();

    world.AddComponent(entity, new Transform());

    Assert.That(world.Query(new EntityQuery().Exclude<Transform>()), Is.Empty);
    Assert.That(world.Query(new EntityQuery().Exclude<Rigidbody>()), Has.Member(entity));
  }

  private static readonly EntityQuery Query = new EntityQuery()
    .Include<Transform>()
    .Include<Rigidbody>();

  private static void SyncTransformManual(in TickEvent @event, EntityWorld world, IPhysicsService service)
  {
    foreach (var entity in world.Query(Query))
    {
      ref var transform = ref world.GetComponent<Transform>(entity);
      ref var rigidbody = ref world.GetComponent<Rigidbody>(entity);

      rigidbody.Velocity += service.GetGravity() * @event.DeltaTime;
      transform.Position += rigidbody.Velocity * @event.DeltaTime;
    }
  }

  private static void SyncTransformAutomatic(in TickEvent @event, ref Transform transform, ref Rigidbody rigidbody, IPhysicsService service)
  {
    rigidbody.Velocity += service.GetGravity() * @event.DeltaTime;
    transform.Position += rigidbody.Velocity * @event.DeltaTime;
  }

  private struct Transform : IComponent<Transform>
  {
    public Vector2 Position { get; set; }
  }

  private struct Rigidbody : IComponent<Rigidbody>
  {
    public Vector2 Velocity { get; set; }
  }

  private interface IPhysicsService
  {
    Vector2 GetGravity();
  }

  private sealed class PhysicsService : IPhysicsService
  {
    public Vector2 GetGravity()
    {
      return new Vector2(0f, -9.81f);
    }
  }
}
