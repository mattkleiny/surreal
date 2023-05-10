namespace Surreal;

public class SceneComponentListTests
{
  [Test]
  public void it_should_notify_when_attaching()
  {
    var node = Substitute.For<ISceneNode>();
    var component = Substitute.For<ISceneComponent>();

    var list = new SceneComponentList(node);

    list.Add(component);

    component.Received().OnAttach(node);
  }

  [Test]
  public void it_should_notify_when_detaching()
  {
    var node = Substitute.For<ISceneNode>();
    var component = Substitute.For<ISceneComponent>();

    var list = new SceneComponentList(node);

    list.Add(component);
    list.Remove(component);

    component.Received().OnDetach(node);
  }
}
