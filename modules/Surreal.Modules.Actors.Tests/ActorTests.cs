using Surreal.Utilities;

namespace Surreal;

public class ActorTests
{
  [Test]
  public void it_should_read_and_write_components_to_internal_storage_group()
  {
    var scene = new ActorScene();
    var actor = new Actor(scene);

    actor.GetOrCreateComponent(new Transform
    {
      Position = Vector2.UnitX,
      Rotation = -2f,
    });
  }
}
