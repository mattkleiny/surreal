using System.Xml.Serialization;
using Surreal.IO;

namespace Surreal.Scenes;

/// <summary>
/// A <see cref="ISceneDefinition"/> for a <see cref="SceneTree"/>.
/// <para/>
/// Allows a <see cref="SceneTree"/> to be instantiated and loaded at runtime,
/// and permits it's use inside of the Editor and Debugging tools.
/// </summary>
[XmlRoot("SceneTree")]
public sealed class SceneTreeDefinition : ISceneDefinition, IJsonSerializable<SceneTreeDefinition>
{
  /// <inheritdoc/>
  public static SceneTreeDefinition FromJson(string json)
  {
    return JsonSerializer.Deserialize<SceneTreeDefinition>(json)!;
  }

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
  /// The root node of the scene.
  /// </summary>
  [XmlElement]
  public SceneNodeDefinition Tree { get; init; } = new();

  /// <inheritdoc/>
  public string ToJson()
  {
    return JsonSerializer.Serialize(this);
  }

  /// <summary>
  /// A single node in a <see cref="SceneTreeDefinition"/>.
  /// </summary>
  public sealed class SceneNodeDefinition
  {
    /// <summary>
    /// The child nodes of this node.
    /// </summary>
    [XmlArray]
    public List<SceneNodeDefinition> Children { get; init; } = new();
  }
}
