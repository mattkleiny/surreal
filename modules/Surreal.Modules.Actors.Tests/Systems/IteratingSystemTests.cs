using NUnit.Framework;
using Surreal.Aspects;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal.Systems;

public class IteratingSystemTests
{
  [Test]
  public void it_should_iterate_over_aspect()
  {
    var scene     = new ActorScene();
    var deltaTime = 16.Milliseconds();

    scene.AddSystem(new TestSystem(scene));

    scene.Input(deltaTime);
    scene.Update(deltaTime);
    scene.Draw(deltaTime);
  }

  private sealed class TestSystem : IteratingSystem
  {
    public TestSystem(IComponentSystemContext context)
      : base(context, Aspect.Of<Transform>())
    {
    }
  }
}
