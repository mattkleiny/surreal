using Surreal.Assets;
using Surreal.Utilities;

namespace Surreal.Scenes;

/// <summary>
/// A <see cref="ISceneDefinition"/> for a <see cref="SceneTree"/>.
/// <para/>
/// Allows a <see cref="SceneTree"/> to be instantiated and loaded at runtime,
/// and permits it's use inside of the Editor and Debugging tools.
/// </summary>
[XmlRoot("SceneTree")]
public sealed class SceneTreeDefinition : SceneNodeDefinition, ISceneDefinition
{
  /// <summary>
  /// Builds a new scene graph and attaches it as a child of the given parent node.
  /// </summary>
  public void BuildSubTree(SceneNode parent)
  {
    var sceneTree = new SceneTree
    {
      // TODO: work out how to resolve these
      Assets = new AssetManager(),
      Services = new ServiceRegistry()
    };

    parent.Add(sceneTree);

    foreach (var child in Children)
    {
      sceneTree.Add(new SceneNode());
    }
  }
}

/// <summary>
/// A single node in a <see cref="SceneTreeDefinition"/>.
/// </summary>
[XmlRoot("SceneNode")]
public class SceneNodeDefinition
{
  /// <summary>
  /// A user-friendly name for the scene.
  /// </summary>
  [XmlAttribute]
  public string? Name { get; set; }

  /// <summary>
  /// An optional description for the scene.
  /// </summary>
  [XmlAttribute]
  public string? Description { get; set; }

  /// <summary>
  /// The child nodes of this node.
  /// </summary>
  [XmlArray]
  public List<SceneNodeDefinition> Children { get; init; } = new();
}
