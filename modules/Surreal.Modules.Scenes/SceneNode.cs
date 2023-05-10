using Surreal.Graphics.Rendering;

namespace Surreal;

/// <summary>
/// A node in a <see cref="SceneGraph"/>.
/// </summary>
public interface ISceneNode : IDisposable
{
  void OnParented(ISceneNode node);
  void OnUnparented();

  void OnEnable();
  void OnDisable();

  void OnUpdate();
  void OnRender(in RenderFrame frame, IRenderContextManager manager);
}

/// <summary>
/// Convenience class for implementing <see cref="ISceneNode"/>.
/// </summary>
public sealed class SceneNode : ISceneNode
{
  private ISceneNode? _parent;

  public SceneNode()
  {
    Children = new SceneNodeList(this);
    Components = new SceneComponentList(this);
  }

  /// <summary>
  /// The current parent of the node.
  /// </summary>
  public ISceneNode? Parent
  {
    get => _parent;
    set
    {
      if (_parent != value)
      {
        _parent = value;

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
  }

  /// <summary>
  /// The child nodes of the node.
  /// </summary>
  public SceneNodeList Children { get; }

  /// <summary>
  /// The components of the node.
  /// </summary>
  public SceneComponentList Components { get; }

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

  public void OnRender(in RenderFrame frame, IRenderContextManager manager)
  {
    foreach (var component in Components)
    {
      component.OnRender(this, in frame, manager);
    }

    foreach (var child in Children)
    {
      child.OnRender(in frame, manager);
    }
  }

  public void Dispose()
  {
    foreach (var component in Components)
    {
      component.Dispose();
    }

    foreach (var child in Children)
    {
      child.Dispose();
    }

    Components.Clear();
  }
}
