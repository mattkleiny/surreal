using Surreal.Assets;
using Surreal.Collections;
using Surreal.Collections.Slices;
using Surreal.Timing;

namespace Surreal.Scenes;

/// <summary>
/// A list of <see cref="SceneNode"/>s.
/// </summary>
[DebuggerDisplay("SceneNodeList (Count = {Count})")]
public sealed class SceneNodeList(SceneNode owner) : Collection<SceneNode>
{
  protected override void OnItemAdded(SceneNode item)
  {
    base.OnItemAdded(item);

    item.OnNodeAdded(owner);
  }

  protected override void OnItemRemoved(SceneNode item)
  {
    base.OnItemRemoved(item);

    item.OnNodeRemoved(owner);
  }
}

/// <summary>
/// A single node in a scene tree.
/// </summary>
public class SceneNode : IEnumerable<SceneNode>, IPropertyChangingEvents, IPropertyChangedEvents, IDisposable
{
  private SceneNodeStates _states = SceneNodeStates.Dormant;
  private ISceneRoot? _root;

  public SceneNode()
  {
    Children = new SceneNodeList(this);
  }

  /// <summary>
  /// Notifies listeners that a property is changing.
  /// </summary>
  public event PropertyEventHandler? PropertyChanging;

  /// <summary>
  /// Notifies listeners that a property has changed.
  /// </summary>
  public event PropertyEventHandler? PropertyChanged;

  /// <summary>
  /// The parent of this node.
  /// </summary>
  public SceneNode? Parent { get; private set; }

  /// <summary>
  /// The children of this node.
  /// </summary>
  public SceneNodeList Children { get; }

  /// <summary>
  /// The root of the entire scene, which provides services and asset access.
  /// </summary>
  public ISceneRoot Root
  {
    get => _root ?? throw new InvalidOperationException("A scene root is not available");
    private set => _root = value;
  }

  /// <summary>
  /// The <see cref="IServiceProvider"/> for the scene tree.
  /// </summary>
  public IServiceProvider Services => Root.Services;

  /// <summary>
  /// The <see cref="IAssetProvider"/> for the scene tree.
  /// </summary>
  public IAssetProvider Assets => Root.Assets;

  public bool IsAwake => _states.HasFlag(SceneNodeStates.Awake);
  public bool IsReady => _states.HasFlag(SceneNodeStates.Ready);
  public bool IsInTree => _states.HasFlag(SceneNodeStates.InTree);
  public bool IsDestroyed => _states.HasFlag(SceneNodeStates.Destroyed);

  // messages that are flowing either to or from this node
  internal Queue<Message> MessagesForParents { get; } = new();
  internal Queue<Message> MessagesForChildren { get; } = new();

  /// <summary>
  /// Convenience method for adding a child to this node.
  /// </summary>
  public void Add(SceneNode node) => Children.Add(node);

  /// <summary>
  /// Convenience method for removing a child from this node.
  /// </summary>
  public void Remove(SceneNode node) => Children.Remove(node);

  /// <summary>
  /// Attempts to find the root <see cref="SceneTree"/> of the hierarchy.
  /// </summary>
  protected virtual bool TryResolveRoot(out ISceneRoot result)
  {
    if (Parent is { _root: { } root })
    {
      result = root;
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
    var current = Parent;

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
    OnAwakeIfNecessary();
    OnEnterTreeIfNecessary();

    DispatchMessagesToChildren();

    OnReadyIfNecessary();

    OnUpdate(deltaTime);

    foreach (var child in Children)
    {
      child.Update(deltaTime);
    }

    DispatchMessagesToParents();
  }

  /// <summary>
  /// Destroys this node and its children.
  /// </summary>
  public void Destroy()
  {
    SendMessageToParents(MessageType.Destroy);
  }

  protected virtual void OnAwake()
  {
  }

  protected virtual void OnReady()
  {
  }

  protected virtual void OnEnterTree()
  {
  }

  protected virtual void OnExitTree()
  {
  }

  protected virtual void OnDestroy()
  {
  }

  protected virtual void OnUpdate(DeltaTime deltaTime)
  {
  }

  internal virtual void OnMessageReceived(Message message)
  {
  }

  internal virtual void OnMessageReceivedFromParent(Message message)
  {
  }

  internal virtual void OnMessageReceivedFromChild(Message message)
  {
  }

  public override string ToString()
  {
    return $"{GetType().Name} ({Children.Count} children)";
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
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  /// <summary>
  /// Notifies this node that it has been added to a scene tree.
  /// </summary>
  internal void OnNodeAdded(SceneNode owner)
  {
    if (Parent != owner)
    {
      Parent?.Children.Remove(this);
      Parent = owner;
    }

    if (owner.IsAwake)
    {
      InitializeInActiveTree();
    }
  }

  /// <summary>
  /// Notifies this node that it has been removed from a scene tree.
  /// </summary>
  internal void OnNodeRemoved(SceneNode owner)
  {
    OnExitTreeIfNecessary();

    Parent = null;
  }

  /// <summary>
  /// Initializes the node and its children in an already active scene tree.
  /// </summary>
  private void InitializeInActiveTree()
  {
    OnAwakeIfNecessary();
    OnEnterTreeIfNecessary();

    DispatchMessagesToChildren();
  }

  /// <summary>
  /// Awakens this node and its children.
  /// </summary>
  private void OnAwakeIfNecessary()
  {
    if (!_states.HasFlagFast(SceneNodeStates.Awake))
    {
      foreach (var child in Children)
      {
        child.OnAwake();
      }

      OnAwake();

      _states |= SceneNodeStates.Awake;
    }
  }

  /// <summary>
  /// Notifies this node that it has been added to a scene tree.
  /// </summary>
  private void OnEnterTreeIfNecessary()
  {
    if (!_states.HasFlagFast(SceneNodeStates.InTree))
    {
      if (TryResolveRoot(out var sceneRoot))
      {
        Root = sceneRoot;
      }

      foreach (var child in Children)
      {
        child.OnEnterTreeIfNecessary();
      }

      OnEnterTree();

      _states |= SceneNodeStates.InTree;
    }
  }

  /// <summary>
  /// Notifies this node that it has been removed from a scene tree.
  /// </summary>
  private void OnExitTreeIfNecessary()
  {
    if (_states.HasFlagFast(SceneNodeStates.InTree))
    {
      foreach (var child in Children)
      {
        child.OnExitTreeIfNecessary();
      }

      OnExitTree();
      Root = null!;

      _states &= ~SceneNodeStates.InTree;
    }
  }

  /// <summary>
  /// Readies this node and its children.
  /// </summary>
  private void OnReadyIfNecessary()
  {
    if (!_states.HasFlagFast(SceneNodeStates.Ready))
    {
      foreach (var child in Children)
      {
        child.OnReadyIfNecessary();
      }

      OnReady();

      _states |= SceneNodeStates.Ready;
    }
  }

  /// <summary>
  /// Notifies this node that it has been destroyed.
  /// </summary>
  internal void OnDestroyIfNecessary()
  {
    if (!_states.HasFlagFast(SceneNodeStates.Destroyed))
    {
      foreach (var child in Children)
      {
        child.OnDestroyIfNecessary();
      }

      OnDestroy();

      _states |= SceneNodeStates.Destroyed;
    }
  }

  /// <summary>
  /// Sends <see cref="Message"/>s up the tree to parents.
  /// </summary>
  private void DispatchMessagesToParents()
  {
    foreach (var child in Children)
    {
      while (child.MessagesForParents.TryDequeue(out var message))
      {
        OnMessageReceived(message);
        OnMessageReceivedFromChild(message);

        if (message.IsRecursive)
        {
          MessagesForParents.Enqueue(message);
        }
      }
    }
  }

  /// <summary>
  /// Sends <see cref="Message"/>s down the tree to children.
  /// </summary>
  private void DispatchMessagesToChildren()
  {
    while (MessagesForChildren.TryDequeue(out var message))
    {
      foreach (var child in Children)
      {
        child.OnMessageReceived(message);
        child.OnMessageReceivedFromParent(message);

        if (message.IsRecursive)
        {
          child.MessagesForChildren.Enqueue(message);
        }
      }
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
  protected void SendMessageToParents(MessageType type, bool recursive = false)
  {
    MessagesForParents.Enqueue(new Message(type, this, recursive));
  }

  /// <summary>
  /// Notifies child nodes of a change.
  /// </summary>
  protected void SendMessageToChildren(MessageType type, bool recursive = false)
  {
    MessagesForChildren.Enqueue(new Message(type, this, recursive));
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
  private enum SceneNodeStates : byte
  {
    Dormant = 0,
    Awake = 1 << 0,
    Ready = 1 << 1,
    InTree = 1 << 2,
    Destroyed = 1 << 3
  }

  /// <summary>
  /// A type of <see cref="Message"/>.
  /// </summary>
  public enum MessageType : byte
  {
    Destroy,
    TransformChanged
  }

  /// <summary>
  /// A message to be processed by a <see cref="SceneNode"/>.
  /// </summary>
  internal readonly record struct Message(
    MessageType Type,
    SceneNode Sender,
    bool IsRecursive
  );
}
