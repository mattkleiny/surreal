using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Scenes;

/// <summary>
/// A list of <see cref="SceneGraphNode"/>s.
/// </summary>
[DebuggerDisplay("SceneNodeList (Count = {Count})")]
public sealed class SceneNodeList(SceneGraphNode? parent = null) : Collection<SceneGraphNode>
{
  protected override void OnItemAdded(SceneGraphNode item)
  {
    base.OnItemAdded(item);

    item.Parent = parent;
    item.NotifyEnterTree();
  }

  protected override void OnItemRemoved(SceneGraphNode item)
  {
    base.OnItemRemoved(item);

    item.NotifyExitTree();
    item.Parent = null;
  }
}

/// <summary>
/// A single node in a scene graph.
/// </summary>
public class SceneGraphNode : IDisposable
{
  private SceneGraphStates _states = SceneGraphStates.Dormant;
  private SceneGraphNode? _parent;

  public SceneGraphNode()
  {
    Children = new SceneNodeList(this);
  }

  /// <summary>
  /// The parent of this node.
  /// </summary>
  public SceneGraphNode? Parent
  {
    get => _parent;
    internal set
    {
      if (_parent != value)
      {
        _parent?.Children.Remove(this);
      }

      _parent = value;
    }
  }

  /// <summary>
  /// A unique identifier for this node.
  /// </summary>
  public Guid Id { get; } = Guid.NewGuid();

  /// <summary>
  /// The children of this node.
  /// </summary>
  public SceneNodeList Children { get; }

  public bool IsAwake => _states.HasFlag(SceneGraphStates.Awake);
  public bool IsReady => _states.HasFlag(SceneGraphStates.Ready);
  public bool IsInTree => _states.HasFlag(SceneGraphStates.InTree);
  public bool IsDestroyed => _states.HasFlag(SceneGraphStates.Destroyed);

  internal Queue<Notification> Inbox { get; } = new();
  internal Queue<Notification> Outbox { get; } = new();

  /// <summary>
  /// Updates this node and its children.
  /// </summary>
  public void Update(DeltaTime deltaTime)
  {
    if (!_states.HasFlagFast(SceneGraphStates.Ready))
    {
      OnReady();

      _states |= SceneGraphStates.Ready;
    }

    OnUpdate(deltaTime);

    // propagate inbox messages to children
    if (Children.Count > 0)
    {
      while (Inbox.TryDequeue(out var notification))
      {
        OnNotification(notification);

        foreach (var child in Children)
        {
          child.Inbox.Enqueue(notification);
        }
      }
    }

    // update children
    foreach (var child in Children)
    {
      child.Update(deltaTime);

      // propagate outbox messages to parents
      while (child.Outbox.TryDequeue(out var notification))
      {
        OnNotification(notification);

        Outbox.Enqueue(notification);
      }
    }
  }

  /// <summary>
  /// Renders this node and its children.
  /// </summary>
  public void Render(DeltaTime deltaTime)
  {
    if (!_states.HasFlagFast(SceneGraphStates.Ready))
    {
      OnReady();

      _states |= SceneGraphStates.Ready;
    }

    OnRender(deltaTime);

    foreach (var child in Children)
    {
      child.Render(deltaTime);
    }
  }

  /// <summary>
  /// Destroys this node and its children.
  /// </summary>
  public void Destroy()
  {
    Outbox.Enqueue(new Notification(NotificationType.Destroy, this));
  }

  protected virtual void OnAwake()
  {
  }

  protected virtual void OnReady()
  {
  }

  protected virtual void OnDestroy()
  {
  }

  protected virtual void OnEnterTree()
  {
  }

  protected virtual void OnExitTree()
  {
  }

  protected virtual void OnUpdate(DeltaTime deltaTime)
  {
  }

  protected virtual void OnRender(DeltaTime deltaTime)
  {
  }

  private void OnNotification(Notification notification)
  {
  }

  public void Dispose()
  {
    foreach (var child in Children)
    {
      child.Dispose();
    }

    Children.Clear();
  }

  public override string ToString()
  {
    return $"SceneGraphNode {{ Id = {Id} }}";
  }

  /// <summary>
  /// Notifies this node that it has been added to a scene graph.
  /// </summary>
  internal void NotifyEnterTree()
  {
    if (!_states.HasFlagFast(SceneGraphStates.Awake))
    {
      OnAwake();

      _states |= SceneGraphStates.Awake;
    }

    if (!_states.HasFlagFast(SceneGraphStates.InTree))
    {
      foreach (var child in Children)
      {
        child.NotifyEnterTree();
      }

      OnEnterTree();

      _states |= SceneGraphStates.InTree;
    }
  }

  /// <summary>
  /// Notifies this node that it has been removed from a scene graph.
  /// </summary>
  internal void NotifyExitTree()
  {
    if (_states.HasFlagFast(SceneGraphStates.InTree))
    {
      foreach (var child in Children)
      {
        child.NotifyExitTree();
      }

      OnExitTree();

      _states &= ~SceneGraphStates.InTree;
    }
  }

  /// <summary>
  /// Notifies this node that it has been destroyed.
  /// </summary>
  internal void NotifyDestroyed()
  {
    if (!_states.HasFlagFast(SceneGraphStates.Destroyed))
    {
      foreach (var child in Children)
      {
        child.NotifyDestroyed();
      }

      OnDestroy();

      _states |= SceneGraphStates.Destroyed;
    }
  }

  /// <summary>
  /// Possible states of a <see cref="SceneGraphNode"/>.
  /// </summary>
  [Flags]
  private enum SceneGraphStates
  {
    Dormant = 0,
    Awake = 1 << 0,
    Ready = 1 << 1,
    InTree = 1 << 2,
    Destroyed = 1 << 3,
  }

  /// <summary>
  /// A type of <see cref="Notification"/>.
  /// </summary>
  internal enum NotificationType : byte
  {
    Destroy
  }

  /// <summary>
  /// A message to be processed by a <see cref="SceneGraphNode"/>.
  /// </summary>
  internal readonly record struct Notification(
    NotificationType Type,
    SceneGraphNode Sender,
    Variant Data = default
  );
}
