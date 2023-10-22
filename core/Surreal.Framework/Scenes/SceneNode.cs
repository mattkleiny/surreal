using Surreal.Assets;
using Surreal.Collections;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal.Scenes;

/// <summary>
/// A list of <see cref="SceneNode"/>s.
/// </summary>
[DebuggerDisplay("SceneNodeList (Count = {Count})")]
public sealed class SceneNodeList(SceneNode? parent = null) : Collection<SceneNode>
{
  protected override void OnItemAdded(SceneNode item)
  {
    base.OnItemAdded(item);

    item.Parent = parent;
    item.NotifyEnterTree();
  }

  protected override void OnItemRemoved(SceneNode item)
  {
    base.OnItemRemoved(item);

    item.NotifyExitTree();
    item.Parent = null;
  }
}

/// <summary>
/// A single node in a scene graph.
/// </summary>
public class SceneNode : IDisposable, IPropertyChangingEvents, IPropertyChangedEvents, IEnumerable<SceneNode>
{
  private SceneGraphStates _states = SceneGraphStates.Dormant;
  private SceneNode? _parent;

  public SceneNode()
  {
    Children = new SceneNodeList(this);
  }

  public event PropertyEventHandler? PropertyChanging;
  public event PropertyEventHandler? PropertyChanged;

  /// <summary>
  /// The parent of this node.
  /// </summary>
  public SceneNode? Parent
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

  /// <summary>
  /// The <see cref="IServiceProvider"/> for the scene graph.
  /// </summary>
  public IServiceProvider Services
  {
    get
    {
      if (!TryResolveParent(out SceneGraph graph))
      {
        throw new InvalidOperationException("Unable to access services from a node that is not in a scene graph.");
      }

      return graph.Services;
    }
  }

  /// <summary>
  /// The <see cref="IAssetProvider"/> for the scene graph.
  /// </summary>
  public IAssetProvider Assets
  {
    get
    {
      if (!TryResolveParent(out SceneGraph graph))
      {
        throw new InvalidOperationException("Unable to access assets from a node that is not in a scene graph.");
      }

      return graph.Assets;
    }
  }

  public bool IsAwake => _states.HasFlag(SceneGraphStates.Awake);
  public bool IsReady => _states.HasFlag(SceneGraphStates.Ready);
  public bool IsInTree => _states.HasFlag(SceneGraphStates.InTree);
  public bool IsDestroyed => _states.HasFlag(SceneGraphStates.Destroyed);

  // notifications that are flowing either to or from this node
  internal Queue<Notification> NotificationsForParents { get; } = new();
  internal Queue<Notification> NotificationsForChildren { get; } = new();

  /// <summary>
  /// Convenience method for adding a child to this node.
  /// </summary>
  public void Add(SceneNode node) => Children.Add(node);

  /// <summary>
  /// Convenience method for removing a child from this node.
  /// </summary>
  public void Remove(SceneNode node) => Children.Remove(node);

  /// <summary>
  /// Attempts to find the parent node of the hierarchy.
  /// </summary>
  public bool TryResolveRoot<T>(out T result)
    where T : SceneNode
  {
    var current = _parent;

    while (current != null)
    {
      if (current.Parent == null)
      {
        break;
      }

      current = current.Parent;
    }

    if (current is T instance)
    {
      result = instance;
      return true;
    }

    result = default!;
    return false;
  }

  /// <summary>
  /// Attempts to find the first parent in the hierarchy of the given type, returning true if found.
  /// </summary>
  public bool TryResolveParent<T>(out T result)
    where T : SceneNode
  {
    var current = _parent;

    while (current != null)
    {
      if (current is T instance)
      {
        result = instance;
        return true;
      }

      current = current.Parent;
    }

    result = default!;
    return false;
  }

  /// <summary>
  /// Resolves all children of the given type recursively.
  /// </summary>
  public ReadOnlySlice<T> ResolveChildren<T>()
    where T : class
  {
    return ResolveChildren<T>(static _ => true);
  }

  /// <summary>
  /// Resolves all children of the given type recursively.
  /// </summary>
  public ReadOnlySlice<T> ResolveChildren<T>(Predicate<T> predicate)
    where T : class
  {
    static void ResolveRecursive(SceneNode node, List<T> results, Predicate<T> predicate, int depth = 0, int maxDepth = 100)
    {
      Debug.Assert(depth < maxDepth);

      foreach (var child in node.Children)
      {
        if (child is T instance)
        {
          if (predicate(instance))
          {
            results.Add(instance);
          }
        }

        ResolveRecursive(child, results, predicate);
      }
    }

    var results = new List<T>();

    ResolveRecursive(this, results, predicate);

    return results;
  }

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

    OnPreUpdate(deltaTime);
    OnUpdate(deltaTime);
    OnPostUpdate(deltaTime);
  }

  /// <summary>
  /// Destroys this node and its children.
  /// </summary>
  public void Destroy()
  {
    NotifyParents(NotificationType.Destroy);
  }

  protected virtual void UpdateInternal(DeltaTime deltaTime)
  {
    if (!_states.HasFlagFast(SceneGraphStates.Ready))
    {
      OnReady();

      _states |= SceneGraphStates.Ready;
    }

    OnUpdate(deltaTime);
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

  protected virtual void OnPreUpdate(DeltaTime deltaTime)
  {
    foreach (var child in Children)
    {
      child.OnPreUpdate(deltaTime);
    }
  }

  protected virtual void OnUpdate(DeltaTime deltaTime)
  {
    // propagate inbox messages to children
    while (NotificationsForChildren.TryDequeue(out var notification))
    {
      OnNotification(notification);

      foreach (var child in Children)
      {
        child.NotificationsForChildren.Enqueue(notification);
      }
    }

    // update children
    foreach (var child in Children)
    {
      child.Update(deltaTime);

      // propagate outbox messages to parents
      while (child.NotificationsForParents.TryDequeue(out var notification))
      {
        OnNotification(notification);

        NotificationsForParents.Enqueue(notification);
      }
    }
  }

  protected virtual void OnPostUpdate(DeltaTime deltaTime)
  {
    foreach (var child in Children)
    {
      child.OnPostUpdate(deltaTime);
    }
  }

  internal virtual void OnNotification(Notification notification)
  {
  }

  public override string ToString()
  {
    return $"{GetType().Name} {{ Id = {Id} }}";
  }

  public void Dispose()
  {
    foreach (var child in Children)
    {
      child.Dispose();
    }

    Children.Clear();
  }

  public List<SceneNode>.Enumerator GetEnumerator()
  {
    return Children.GetEnumerator();
  }

  IEnumerator<SceneNode> IEnumerable<SceneNode>.GetEnumerator()
  {
    throw new NotImplementedException();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
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
  /// Sets a field and notifies listeners if the value has changed.
  /// </summary>
  protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
  {
    return SetField(ref field, value, EqualityComparer<T>.Default, propertyName);
  }

  /// <summary>
  /// Sets a field and notifies listeners if the value has changed.
  /// </summary>
  protected bool SetField<T>(ref T field, T value, EqualityComparer<T> comparer, [CallerMemberName] string propertyName = "")
  {
    if (!comparer.Equals(field, value))
    {
      NotifyPropertyChanging(propertyName);
      field = value;
      NotifyPropertyChanged(propertyName);

      return true;
    }

    return false;
  }

  /// <summary>
  /// Notifies the parent nodes of a change.
  /// </summary>
  protected void NotifyParents(NotificationType type)
  {
    NotificationsForParents.Enqueue(new Notification(type, this));
  }

  /// <summary>
  /// Notifies child nodes of a change.
  /// </summary>
  protected void NotifyChildren(NotificationType type)
  {
    NotificationsForChildren.Enqueue(new Notification(type, this));
  }

  /// <summary>
  /// Notifies listeners that a property has changed.
  /// </summary>
  protected virtual void NotifyPropertyChanging([CallerMemberName] string propertyName = "")
  {
    PropertyChanging?.Invoke(this, propertyName);
  }

  /// <summary>
  /// Notifies listeners that a property has changed.
  /// </summary>
  protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, propertyName);
  }

  /// <summary>
  /// Possible states of a <see cref="SceneNode"/>.
  /// </summary>
  [Flags]
  private enum SceneGraphStates
  {
    Dormant = 0,
    Awake = 1 << 0,
    Ready = 1 << 1,
    InTree = 1 << 2,
    Destroyed = 1 << 3
  }

  /// <summary>
  /// A type of <see cref="Notification"/>.
  /// </summary>
  public enum NotificationType : byte
  {
    Destroy,
    TransformChanged
  }

  /// <summary>
  /// A message to be processed by a <see cref="SceneNode"/>.
  /// </summary>
  internal readonly record struct Notification(NotificationType Type, SceneNode Sender);
}
