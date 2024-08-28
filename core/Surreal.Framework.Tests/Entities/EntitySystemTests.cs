using Surreal.Scenes;
using Surreal.Timing;

namespace Surreal.Entities;

public class EntitySystemTests
{
  [Test]
  public void it_should_work()
  {
    var world = new EntityWorld();
    var entity = world.SpawnEntity();

    world.AddComponent(entity, new Transform { Position = new(0f, 10f) });
    world.AddComponent(entity, new Rigidbody { Gravity = new(0f, -9.81f) });

    world.AddSystem<TickEvent>(SyncTransformToRigidbody);

    world.Execute(new TickEvent(DeltaTime.Default));
  }

  private static void SyncTransformToRigidbody(in TickEvent @event, ref Transform transform, Rigidbody rigidbody)
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
