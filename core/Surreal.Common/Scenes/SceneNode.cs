namespace Surreal.Scenes;

/// <summary>
/// A node in a <see cref="SceneGraph"/>.
/// </summary>
public interface ISceneNode
{
  void OnParented(ISceneNode node);
  void OnUnparented();

  void OnEnable();
  void OnDisable();

  void OnUpdate();
  void OnRender();
}

/// <summary>
/// Convenience class for implementing <see cref="ISceneNode"/>.
/// </summary>
public sealed class SceneNode : IDisposable, ISceneNode
{
  private ISceneNode? _parent;

  public SceneNode()
  {
    Children = new SceneNodeCollection(this);
    Components = new SceneComponentCollection(this);
  }

  /// <summary>
  /// The current parent of the node.
  /// </summary>
  public ISceneNode? Parent
  {
    get => _parent;
    set
    {
      if (_parent == value) return;

      if (value != null)
      {
        OnParented(value);
      }
      else
      {
        OnUnparented();
      }
    }
  }

  /// <summary>
  /// The child nodes of the node.
  /// </summary>
  public SceneNodeCollection Children { get; }

  /// <summary>
  /// The components of the node.
  /// </summary>
  public SceneComponentCollection Components { get; }

  public void OnParented(ISceneNode node)
  {
    if (Parent != null)
    {
      OnUnparented();
    }

    Parent = node;
  }

  public void OnUnparented()
  {
    Parent = null;
  }

  public void OnEnable()
  {
    foreach (var component in Components)
    {
      component.OnEnable(this);
    }

    foreach (var child in Children)
    {
      child.OnEnable();
    }
  }

  public void OnDisable()
  {
    foreach (var component in Components)
    {
      component.OnDisable(this);
    }

    foreach (var child in Children)
    {
      child.OnDisable();
    }
  }

  public void OnUpdate()
  {
    foreach (var component in Components)
    {
      component.OnUpdate(this);
    }

    foreach (var child in Children)
    {
      child.OnUpdate();
    }
  }

  public void OnRender()
  {
    foreach (var component in Components)
    {
      component.OnRender(this);
    }

    foreach (var child in Children)
    {
      child.OnRender();
    }
  }

  public void Dispose()
  {
    foreach (var component in Components)
    {
      if (component is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    Components.Clear();
  }
}
