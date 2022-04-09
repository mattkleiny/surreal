using System.Reflection;
using Surreal.Components;
using Surreal.Systems;
using Surreal.Timing;

namespace Surreal;

/// <summary>A scene of managed <see cref="Actor"/>s.</summary>
public sealed class ActorScene : IActorContext, IComponentSystemContext, IDisposable
{
  private readonly Dictionary<ActorId, Node<Actor>> nodes = new();
  private readonly ComponentStorageGroup components = new();
  private readonly LinkedList<IComponentSystem> systems = new();
  private readonly Queue<Actor> destroyQueue = new();

  private ulong nextActorId = 0;

  // TODO: find a way to fire these?
  public event ComponentChangeListener? ComponentAdded;
  public event ComponentChangeListener? ComponentRemoved;

  public void AddSystem(IComponentSystem system)
  {
    systems.AddLast(system);
  }

  public void RemoveSystem(IComponentSystem system)
  {
    systems.Remove(system);
  }

  public void Spawn(Actor actor)
  {
    actor.Connect(this);

    if (nodes.TryGetValue(actor.Id, out var node))
    {
      node.ActorStatus = ActorStatus.Active;
    }
    else
    {
      nodes[actor.Id] = new Node<Actor>(actor)
      {
        ActorStatus = ActorStatus.Active,
      };

      // TODO: split these up, better FSM over actors
      actor.OnEnable();
      actor.OnAwake();
      actor.OnStart();
    }
  }

  public void Input(DeltaTime time)
  {
    foreach (var system in systems)
    {
      system.OnInput(time);
    }

    foreach (var node in nodes.Values)
    {
      if (node.ActorStatus == ActorStatus.Active)
      {
        node.Data.OnInput(time);
      }
    }
  }

  public void Update(DeltaTime time)
  {
    foreach (var system in systems)
    {
      system.OnUpdate(time);
    }

    foreach (var node in nodes.Values)
    {
      if (node.ActorStatus == ActorStatus.Active)
      {
        node.Data.OnUpdate(time);
      }
    }

    ProcessDestroyQueue();
  }

  public void Draw(DeltaTime time)
  {
    foreach (var system in systems)
    {
      system.OnDraw(time);
    }

    foreach (var node in nodes.Values)
    {
      if (node.ActorStatus == ActorStatus.Active)
      {
        node.Data.OnDraw(time);
      }
    }
  }

  public ActorStatus GetStatus(ActorId id)
  {
    if (nodes.TryGetValue(id, out var node))
    {
      return node.ActorStatus;
    }

    return ActorStatus.Unknown;
  }

  ActorId IActorContext.AllocateId()
  {
    return new ActorId(Interlocked.Increment(ref nextActorId));
  }

  void IActorContext.Enable(ActorId id)
  {
    if (nodes.TryGetValue(id, out var node) && node.ActorStatus != ActorStatus.Destroyed)
    {
      node.ActorStatus = ActorStatus.Active;
    }
  }

  void IActorContext.Disable(ActorId id)
  {
    if (nodes.TryGetValue(id, out var node) && node.ActorStatus != ActorStatus.Destroyed)
    {
      node.ActorStatus = ActorStatus.Inactive;
    }
  }

  void IActorContext.Destroy(ActorId id)
  {
    if (nodes.TryGetValue(id, out var node) && node.ActorStatus != ActorStatus.Destroyed)
    {
      node.ActorStatus = ActorStatus.Destroyed;

      destroyQueue.Enqueue(node.Data);
    }
  }

  private void ProcessDestroyQueue()
  {
    while (destroyQueue.TryDequeue(out var actor))
    {
      // TODO: split these up, better FSM over actors
      actor.OnDisable();
      actor.OnDestroy();

      actor.Disconnect(this);

      nodes.Remove(actor.Id);
      components.RemoveAll(actor.Id);
    }
  }

  public void Dispose()
  {
    components.Dispose();
    nodes.Clear();
  }

  IComponentStorage<T> IActorContext.GetStorage<T>()
  {
    return components.GetOrCreateStorage<T>();
  }

  /// <summary>A single node in the scene.</summary>
  private sealed record Node<T>(T Data)
  {
    public ActorStatus   ActorStatus { get; set; }
    public Node<T>?      Parent      { get; set; }
    public List<Node<T>> Children    { get; } = new();
  }

  /// <summary>A storage group for components, for use in scene actors.</summary>
  private sealed class ComponentStorageGroup : IDisposable
  {
    private readonly Dictionary<Type, IComponentStorage> storagesByType = new();

    public IComponentStorage<T> GetOrCreateStorage<T>()
      where T : notnull, new()
    {
      if (!storagesByType.TryGetValue(typeof(T), out var storage))
      {
        storagesByType[typeof(T)] = storage = CreateStorage<T>();
      }

      return (IComponentStorage<T>) storage;
    }

    public void RemoveAll(ActorId id)
    {
      foreach (var storage in storagesByType.Values)
      {
        storage.RemoveComponent(id);
      }
    }

    public void Dispose()
    {
      foreach (var storage in storagesByType.Values)
      {
        if (storage is IDisposable disposable)
        {
          disposable.Dispose();
        }
      }

      storagesByType.Clear();
    }

    private static IComponentStorage<T> CreateStorage<T>()
      where T : notnull, new()
    {
      var storageType = typeof(SparseComponentStorage<>);
      var attribute = typeof(T).GetCustomAttribute<ComponentAttribute>();

      // check for per-component storage types
      if (attribute != null)
      {
        storageType = attribute.StorageType ?? storageType;
      }

      var instance = Activator.CreateInstance(storageType.MakeGenericType(typeof(T)))!;

      return (IComponentStorage<T>) instance;
    }
  }
}
