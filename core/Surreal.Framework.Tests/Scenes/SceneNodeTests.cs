using Surreal.Assets;
using Surreal.Timing;

namespace Surreal.Scenes;

public class SceneNodeTests
{
  [Test]
  public void it_should_notify_node_of_attachment_to_tree()
  {
    var scene = new SceneTree
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>()
    };
    var node = new SceneNode();

    node.IsInTree.Should().BeFalse();

    scene.Add(node);

    node.IsInTree.Should().BeTrue();
  }

  [Test]
  public void it_should_notify_node_of_removal_from_tree()
  {
    var scene = new SceneTree
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>()
    };

    var node = new SceneNode();

    scene.Add(node);

    node.IsInTree.Should().BeTrue();

    scene.Remove(node);

    node.IsInTree.Should().BeFalse();
  }

  [Test]
  public void it_should_awake_node_on_first_attachment_to_tree()
  {
    var scene = new SceneTree
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>()
    };

    var node = new SceneNode();

    node.IsAwake.Should().BeFalse();

    scene.Add(node);

    node.IsAwake.Should().BeTrue();

    scene.Remove(node);

    node.IsAwake.Should().BeTrue();
  }

  [Test]
  public void it_should_notify_ready_on_first_update()
  {
    var scene = new SceneTree
    {
      Assets = Substitute.For<IAssetProvider>(),
      Services = Substitute.For<IServiceProvider>()
    };
    var node = new SceneNode();

    scene.Add(node);

    node.IsReady.Should().BeFalse();

    scene.Update(DeltaTime.OneOver60);

    node.IsReady.Should().BeTrue();
  }

  [Test]
  public void it_should_propagate_destruction_up_to_root()
  {
    var scene = new SceneTree
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

    scene.Update(DeltaTime.OneOver60);
  }

  [Test]
  public void it_should_propagate_disposal_down_to_children()
  {
    var scene = new SceneTree
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
