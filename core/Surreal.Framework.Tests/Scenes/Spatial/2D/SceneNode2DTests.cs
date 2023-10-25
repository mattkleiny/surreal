using Surreal.Maths;
using Surreal.Scenes.Spatial;
using Surreal.Timing;

namespace Surreal.Framework.Tests.Scenes.Spatial;

public class SceneNode2DTests
{
  [Test]
  public void it_should_propagate_global_transform_changes()
  {
    var child1 = new TestNode2D();
    var child2 = new TestNode2D
    {
      LocalPosition = Vector2.One
    };

    var node = new TestNode2D
    {
      GlobalPosition = Vector2.One,
      GlobalRotation = Angle.FromDegrees(45f),
      Children = { child1, child2 }
    };

    node.Update(DeltaTime.OneOver60);

    child1.GlobalPosition.Should().Be(Vector2.One);
    child1.GlobalRotation.Should().Be(Angle.FromDegrees(45f));
    child2.GlobalPosition.Should().Be(Vector2.One * 2f);
    child2.GlobalRotation.Should().Be(Angle.FromDegrees(45f));

    node.GlobalPosition = -Vector2.One * 4f;

    node.Update(DeltaTime.OneOver60);

    child1.GlobalPosition.Should().Be(-Vector2.One * 4f);
    child1.GlobalRotation.Should().Be(Angle.FromDegrees(45f));
    child2.GlobalPosition.Should().Be(-Vector2.One * 3f);
    child2.GlobalRotation.Should().Be(Angle.FromDegrees(45f));
  }

  private sealed class TestNode2D: Node2D
  {
    public void Update(DeltaTime deltaTime)
    {
      OnUpdate(deltaTime);
    }
  }
}
