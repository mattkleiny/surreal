using Surreal.Mathematics;
using Surreal.Timing;

namespace Surreal.Scenes.Canvas;

public class SceneNode2DTests
{
  [Test]
  public void it_should_propagate_global_transform_changes()
  {
    var child1 = new Node2D();
    var child2 = new Node2D
    {
      LocalPosition = Vector2.One
    };

    var node = new Node2D
    {
      GlobalPosition = Vector2.One,
      GlobalRotation = Angle.FromDegrees(45f),
      Children = { child1, child2 }
    };

    node.Update(DeltaTime.Default);

    child1.GlobalPosition.Should().Be(Vector2.One);
    child1.GlobalRotation.Should().Be(Angle.FromDegrees(45f));
    child2.GlobalPosition.Should().Be(Vector2.One * 2f);
    child2.GlobalRotation.Should().Be(Angle.FromDegrees(45f));

    node.GlobalPosition = -Vector2.One * 4f;

    node.Update(DeltaTime.Default);

    child1.GlobalPosition.Should().Be(-Vector2.One * 4f);
    child1.GlobalRotation.Should().Be(Angle.FromDegrees(45f));
    child2.GlobalPosition.Should().Be(-Vector2.One * 3f);
    child2.GlobalRotation.Should().Be(Angle.FromDegrees(45f));
  }
}
