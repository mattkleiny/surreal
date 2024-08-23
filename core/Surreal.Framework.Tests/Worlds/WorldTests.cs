using Surreal.Services;
using Surreal.Timing;

namespace Surreal.Worlds;

public class WorldTests
{
  [Test]
  public void it_should_work()
  {
    var services = new ServiceRegistry();
    var world = new World(services);
    var entity = world.Spawn();

    world.AddComponent(entity, new Transform());

    world.AddSystem<TickEvent>((ref Transform transform) =>
    {
      Console.WriteLine($"Transform: {transform}");

      transform.Position += Vector3.UnitY;
    });

    world.Publish(new TickEvent(DeltaTime.Default));

    ref var transform = ref world.GetComponent<Transform>(entity);

    transform.Position.Should().Be(Vector3.UnitY);
  }

  private record struct Transform : IComponent<Transform>
  {
    public static IComponentStorage<Transform> CreateStorage()
    {
      return new SparseComponentStorage<Transform>();
    }

    public Vector3 Position { get; set; }
  }
}
