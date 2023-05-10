using Surreal.Graphics.Rendering;
using Surreal.Timing;

namespace Surreal;

public class SceneNodeTest
{
  [Test]
  public void it_should_notify_child_nodes_of_enable()
  {
    var child1 = Substitute.For<ISceneNode>();
    var child2 = Substitute.For<ISceneNode>();

    var node = new SceneNode { Children = { child1, child2 } };

    node.OnEnable();

    child1.Received().OnEnable();
    child2.Received().OnEnable();
  }

  [Test]
  public void it_should_notify_child_nodes_of_disable()
  {
    var child1 = Substitute.For<ISceneNode>();
    var child2 = Substitute.For<ISceneNode>();

    var node = new SceneNode { Children = { child1, child2 } };

    node.OnDisable();

    child1.Received().OnDisable();
    child2.Received().OnDisable();
  }

  [Test]
  public void it_should_notify_child_nodes_of_update()
  {
    var child1 = Substitute.For<ISceneNode>();
    var child2 = Substitute.For<ISceneNode>();

    var node = new SceneNode { Children = { child1, child2 } };

    node.OnUpdate();

    child1.Received().OnUpdate();
    child2.Received().OnUpdate();
  }

  [Test]
  public void it_should_notify_child_nodes_of_render()
  {
    var child1 = Substitute.For<ISceneNode>();
    var child2 = Substitute.For<ISceneNode>();

    var node = new SceneNode { Children = { child1, child2 } };
    var manager = Substitute.For<IRenderContextManager>();
    var frame = new RenderFrame { DeltaTime = TimeDelta.Default };

    node.OnRender(in frame, manager);

    child1.Received().OnRender(in frame, manager);
    child2.Received().OnRender(in frame, manager);
  }

  [Test]
  public void it_should_notify_child_components_of_enable()
  {
    var component1 = Substitute.For<ISceneComponent>();
    var component2 = Substitute.For<ISceneComponent>();

    var node = new SceneNode { Components = { component1, component2 } };

    node.OnEnable();

    component1.Received().OnEnable(node);
    component2.Received().OnEnable(node);
  }

  [Test]
  public void it_should_notify_component_components_of_disable()
  {
    var component1 = Substitute.For<ISceneComponent>();
    var component2 = Substitute.For<ISceneComponent>();

    var node = new SceneNode { Components = { component1, component2 } };

    node.OnDisable();

    component1.Received().OnDisable(node);
    component2.Received().OnDisable(node);
  }

  [Test]
  public void it_should_notify_component_components_of_update()
  {
    var component1 = Substitute.For<ISceneComponent>();
    var component2 = Substitute.For<ISceneComponent>();

    var node = new SceneNode { Components = { component1, component2 } };

    node.OnUpdate();

    component1.Received().OnUpdate(node);
    component2.Received().OnUpdate(node);
  }

  [Test]
  public void it_should_notify_component_components_of_render()
  {
    var component1 = Substitute.For<ISceneComponent>();
    var component2 = Substitute.For<ISceneComponent>();

    var node = new SceneNode { Components = { component1, component2 } };
    var manager = Substitute.For<IRenderContextManager>();
    var frame = new RenderFrame { DeltaTime = TimeDelta.Default };

    node.OnRender(in frame, manager);

    component1.Received().OnRender(node, in frame, manager);
    component2.Received().OnRender(node, in frame, manager);
  }
}
