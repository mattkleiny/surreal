using Surreal.Timing;
using static Surreal.Scenes.SceneGraphNode;

namespace Surreal.Scenes;

public class SceneGraphNodeTests
{
  [Test]
  public void it_should_notify_node_of_attachment_to_tree()
  {
    var scene = new SceneGraphScene();
    var node = new SceneGraphNode();

    node.IsInTree.Should().BeFalse();

    scene.Root.Children.Add(node);

    node.IsInTree.Should().BeTrue();
  }

  [Test]
  public void it_should_notify_node_of_removal_from_tree()
  {
    var scene = new SceneGraphScene();
    var node = new SceneGraphNode();

    scene.Root.Children.Add(node);

    node.IsInTree.Should().BeTrue();

    scene.Root.Children.Remove(node);

    node.IsInTree.Should().BeFalse();
  }

  [Test]
  public void it_should_awake_node_on_first_attachment_to_tree()
  {
    var scene = new SceneGraphScene();
    var node = new SceneGraphNode();

    node.IsAwake.Should().BeFalse();

    scene.Root.Children.Add(node);

    node.IsAwake.Should().BeTrue();

    scene.Root.Children.Remove(node);

    node.IsAwake.Should().BeTrue();
  }

  [Test]
  public void it_should_notify_ready_on_first_update()
  {
    var scene = new SceneGraphScene();
    var node = new SceneGraphNode();

    scene.Root.Children.Add(node);

    node.IsReady.Should().BeFalse();

    scene.Tick(DeltaTime.OneOver60);

    node.IsReady.Should().BeTrue();
  }

  [Test]
  public void it_should_propagate_inbox_messages_to_children()
  {
    var parent = new SceneGraphNode();
    var child1 = new SceneGraphNode();
    var child2 = new SceneGraphNode();

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
    var parent = new SceneGraphNode();
    var child = new SceneGraphNode();

    parent.Children.Add(child);

    child.Outbox.Enqueue(new Notification(NotificationType.Destroy, child));

    parent.Update(DeltaTime.OneOver60);

    parent.Outbox.Should().ContainSingle();
    parent.Outbox.Dequeue().Type.Should().Be(NotificationType.Destroy);
  }

  [Test]
  public void it_should_propagate_destruction_up_to_root()
  {
    var scene = new SceneGraphScene
    {
      Root =
      {
        Children =
        {
          new SceneGraphNode
          {
            Children =
            {
              new SceneGraphNode(),
              new SceneGraphNode()
            }
          },
          new SceneGraphNode
          {
            Children = { new SceneGraphNode() }
          }
        }
      }
    };

    scene.Root.Children[0].Destroy();

    scene.Tick(DeltaTime.OneOver60);
  }
}
