using Surreal.Assets;
using Surreal.Timing;
using static Surreal.Scenes.SceneNode;

namespace Surreal.Scenes;

public class SceneNodeTests
{
  [Test]
  public void it_should_notify_node_of_attachment_to_tree()
  {
    var scene = new SceneGraph
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>()
    };
    var node = new SceneNode();

    node.IsInTree.Should().BeFalse();

    scene.Children.Add(node);

    node.IsInTree.Should().BeTrue();
  }

  [Test]
  public void it_should_notify_node_of_removal_from_tree()
  {
    var scene = new SceneGraph
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>()
    };

    var node = new SceneNode();

    scene.Children.Add(node);

    node.IsInTree.Should().BeTrue();

    scene.Children.Remove(node);

    node.IsInTree.Should().BeFalse();
  }

  [Test]
  public void it_should_awake_node_on_first_attachment_to_tree()
  {
    var scene = new SceneGraph
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>()
    };

    var node = new SceneNode();

    node.IsAwake.Should().BeFalse();

    scene.Children.Add(node);

    node.IsAwake.Should().BeTrue();

    scene.Children.Remove(node);

    node.IsAwake.Should().BeTrue();
  }

  [Test]
  public void it_should_notify_ready_on_first_update()
  {
    var scene = new SceneGraph
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>()
    };
    var node = new SceneNode();

    scene.Children.Add(node);

    node.IsReady.Should().BeFalse();

    ((SceneNode)scene).Update(DeltaTime.OneOver60);

    node.IsReady.Should().BeTrue();
  }

  [Test]
  [Ignore("Find a better way to capture notifications for testing")]
  public void it_should_propagate_inbox_messages_to_children()
  {
    var parent = new SceneNode();
    var child1 = new SceneNode();
    var child2 = new SceneNode();

    parent.Children.Add(child1);
    parent.Children.Add(child2);

    parent.Inbox.Enqueue(new Notification(NotificationType.Destroy, parent));

    parent.Update(DeltaTime.OneOver60);

    child1.Inbox.Should().ContainSingle();
    child1.Inbox.Dequeue().Type.Should().Be(NotificationType.Destroy);

    child2.Inbox.Should().ContainSingle();
    child2.Inbox.Dequeue().Type.Should().Be(NotificationType.Destroy);
  }

  [Test]
  public void it_should_propagate_outbox_message_to_parent()
  {
    var parent = new SceneNode();
    var child = new SceneNode();

    parent.Children.Add(child);

    child.Outbox.Enqueue(new Notification(NotificationType.Destroy, child));

    parent.Update(DeltaTime.OneOver60);

    parent.Outbox.Should().ContainSingle();
    parent.Outbox.Dequeue().Type.Should().Be(NotificationType.Destroy);
  }

  [Test]
  public void it_should_propagate_destruction_up_to_root()
  {
    var scene = new SceneGraph
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>(),
    };

    scene.Add(new SceneNode
    {
      Children =
      {
        new SceneNode(),
        new SceneNode()
      }
    });

    scene.Add(new SceneNode
    {
      Children = { new SceneNode() }
    });

    scene.Children[0].Destroy();

    ((SceneNode)scene).Update(DeltaTime.OneOver60);
  }

  [Test]
  public void it_should_propagate_disposal_down_to_children()
  {
    var scene = new SceneGraph
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>()
    };

    scene.Add(new SceneNode
    {
      Children =
      {
        new SceneNode(),
        new SceneNode()
      }
    });

    scene.Add(new SceneNode
    {
      Children = { new SceneNode() }
    });

    scene.Dispose();
  }
}
