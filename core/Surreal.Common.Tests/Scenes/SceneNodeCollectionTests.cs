namespace Surreal.Scenes;

public class SceneNodeCollectionTests
{
  [Test]
  public void it_should_notify_when_parenting()
  {
    var parent = Substitute.For<ISceneNode>();
    var child = Substitute.For<ISceneNode>();

    var collection = new SceneNodeCollection(parent);

    collection.Add(child);

    child.Received().OnParented(parent);
  }

  [Test]
  public void it_should_notify_when_unparenting()
  {
    var parent = Substitute.For<ISceneNode>();
    var child = Substitute.For<ISceneNode>();

    var collection = new SceneNodeCollection(parent);

    collection.Add(child);
    collection.Remove(child);

    child.Received().OnUnparented();
  }
}
