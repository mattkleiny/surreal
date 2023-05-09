namespace Surreal.Scenes;

public class SceneComponentCollectionTests
{
  [Test]
  public void it_should_notify_when_attaching()
  {
    var node = Substitute.For<ISceneNode>();
    var component = Substitute.For<ISceneComponent>();

    var collection = new SceneComponentCollection(node);

    collection.Add(component);

    component.Received().OnAttach(node);
  }

  [Test]
  public void it_should_notify_when_detaching()
  {
    var node = Substitute.For<ISceneNode>();
    var component = Substitute.For<ISceneComponent>();

    var collection = new SceneComponentCollection(node);

    collection.Add(component);
    collection.Remove(component);

    component.Received().OnDetach(node);
  }
}
