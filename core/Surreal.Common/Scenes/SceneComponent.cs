namespace Surreal.Scenes;

/// <summary>
/// A component that can be attached to a <see cref="SceneNode"/>.
/// </summary>
public interface ISceneComponent
{
  /// <summary>
  /// Invoked when the component is attached to a <see cref="SceneNode"/>.
  /// </summary>
  void OnAttach(ISceneNode node);

  /// <summary>
  /// Invoked when the component is detached from a <see cref="SceneNode"/>.
  /// </summary>
  void OnDetach(ISceneNode node);

  /// <summary>
  /// Invoked when the component is enabled.
  /// </summary>
  void OnEnable(ISceneNode node);

  /// <summary>
  /// Invoked when the component is disabled.
  /// </summary>
  void OnDisable(ISceneNode node);

  /// <summary>
  /// Invoked when the component is updated.
  /// </summary>
  void OnUpdate(ISceneNode node);

  /// <summary>
  /// Invoked when the component is rendered.
  /// </summary>
  /// <param name="node"></param>
  void OnRender(ISceneNode node);
}

/// <summary>
/// Convenience class for <see cref="ISceneComponent"/>.
/// </summary>
public abstract class SceneComponent : ISceneComponent
{
  public ISceneNode? Node { get; private set; }

  public virtual void OnAttach(ISceneNode node)
  {
    if (Node != null)
    {
      OnDetach(Node);
    }

    Node = node;
  }

  public virtual void OnDetach(ISceneNode node)
  {
    Node = null;
  }

  public virtual void OnEnable(ISceneNode node)
  {
  }

  public virtual void OnDisable(ISceneNode node)
  {
  }

  public virtual void OnUpdate(ISceneNode node)
  {
  }

  public virtual void OnRender(ISceneNode node)
  {
  }
}
