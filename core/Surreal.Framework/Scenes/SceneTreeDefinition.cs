namespace Surreal.Scenes;

/// <summary>
/// A <see cref="ISceneDefinition"/> for a <see cref="SceneTree"/>.
/// <para/>
/// Allows a <see cref="SceneTree"/> to be instantiated and loaded at runtime,
/// and permits it's use inside of the Editor and Debugging tools for introspection.
/// </summary>
[XmlRoot("SceneTree")]
public sealed class SceneTreeDefinition : SceneNodeDefinition, ISceneDefinition
{
  /// <summary>
  /// Builds a new scene graph and attaches it as a child of the given parent node.
  /// </summary>
  public void BuildSubTree(SceneNode parent)
  {
    // TODO: implement me
  }
}
