using NUnit.Framework;
using Surreal.Utilities;

namespace Surreal;

public class ActorTests
{
  [Test]
  public void it_should_read_and_write_components()
  {
    var scene = new ActorScene();
    var actor = new Actor(scene);

    ref var transform = ref actor.GetOrCreateComponent(new Transform
    {
      Position = Vector2.UnitX,
      Rotation = -2f
    });

    scene.Spawn(actor);

    transform.Position += Vector2.One;
    transform.Rotation += 4 * MathF.PI;

    actor.RemoveComponent<Transform>();
  }
}
