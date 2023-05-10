namespace Surreal;

public class SceneNodeListTests
{
  [Test]
  public void it_should_notify_when_parenting()
  {
    var parent = Substitute.For<ISceneNode>();
    var child = Substitute.For<ISceneNode>();

    var list = new SceneNodeList(parent);

    list.Add(child);

    child.Received().OnParented(parent);
  }

  [Test]
  public void it_should_notify_when_unparenting()
  {
    var parent = Substitute.For<ISceneNode>();
    var child = Substitute.For<ISceneNode>();

    var list = new SceneNodeList(parent);

    list.Add(child);
    list.Remove(child);

    child.Received().OnUnparented();
  }
}
