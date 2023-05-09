namespace Surreal.Scenes;

/// <summary>
/// A component that can be attached to a <see cref="SceneNode"/>.
/// </summary>
public interface ISceneComponent
{
  void OnAttach(ISceneNode node);
  void OnDetach(ISceneNode node);

  void OnEnable(ISceneNode node);
  void OnDisable(ISceneNode node);

  void OnUpdate(ISceneNode node);
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
