namespace Surreal.Scenes;

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
  public List<SceneNodeDefinition> Children { get; init; } = [];
}
