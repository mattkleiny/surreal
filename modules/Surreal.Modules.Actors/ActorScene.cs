using System.Reflection;
using Surreal.Aspects;
using Surreal.Storage;
using Surreal.Systems;
using Surreal.Timing;

namespace Surreal;

/// <summary>A scene of managed <see cref="Actor"/>s.</summary>
public sealed class ActorScene : IActorContext, IComponentSystemContext, IDisposable
{
  private readonly Dictionary<ActorId, Node<Actor>>    nodes    = new();
  private readonly Dictionary<Type, IComponentStorage> storages = new();
  private readonly LinkedList<IComponentSystem>        systems  = new();

  private readonly Queue<Actor> destroyQueue = new();

  public void AddSystem(IComponentSystem system)    => systems.AddLast(system);
  public void RemoveSystem(IComponentSystem system) => systems.Remove(system);

  public void Input(DeltaTime time)
  {
    foreach (var system in systems)
    {
      system.OnInput(time);
    }

    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
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
      if (node.Status == ActorStatus.Active)
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
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnDraw(time);
      }
    }
  }

  public ActorStatus GetStatus(ActorId id)
  {
    if (nodes.TryGetValue(id, out var node))
    {
      return node.Status;
    }

    return ActorStatus.Unknown;
  }

  public void Spawn(Actor actor)
  {
    if (nodes.TryGetValue(actor.Id, out var node))
    {
      node.Status = ActorStatus.Active;
    }
    else
    {
      nodes[actor.Id] = new Node<Actor>(actor)
      {
        Status = ActorStatus.Active,
      };

      actor.OnAwake();
    }
  }

  void IActorContext.Enable(ActorId id)
  {
    if (nodes.TryGetValue(id, out var node) && node.Status != ActorStatus.Destroyed)
    {
      node.Status = ActorStatus.Active;
    }
  }

  void IActorContext.Disable(ActorId id)
  {
    if (nodes.TryGetValue(id, out var node) && node.Status != ActorStatus.Destroyed)
    {
      node.Status = ActorStatus.Inactive;
    }
  }

  void IActorContext.Destroy(ActorId id)
  {
    if (nodes.TryGetValue(id, out var node) && node.Status != ActorStatus.Destroyed)
    {
      node.Status = ActorStatus.Destroyed;

      destroyQueue.Enqueue(node.Data);
    }
  }

  IComponentStorage<T> IActorContext.GetStorage<T>()
  {
    if (!storages.TryGetValue(typeof(T), out var storage))
    {
      storages[typeof(T)] = storage = CreateStorage<T>();
    }

    return (IComponentStorage<T>) storage;
  }

  AspectEnumerator IComponentSystemContext.QueryActors(Aspect aspect)
  {
    throw new NotImplementedException();
  }

  private void ProcessDestroyQueue()
  {
    while (destroyQueue.TryDequeue(out var actor))
    {
      actor.OnDestroy();

      nodes.Remove(actor.Id);
    }
  }

  private static IComponentStorage<T> CreateStorage<T>()
    where T : notnull
  {
    var storageType = typeof(SparseComponentStorage<>);

    // check for per-component storage types
    var attribute = typeof(T).GetCustomAttribute<ComponentAttribute>();
    if (attribute != null)
    {
      storageType = attribute.StorageType ?? storageType;
    }

    var instance = Activator.CreateInstance(storageType.MakeGenericType(typeof(T)))!;

    return (IComponentStorage<T>) instance;
  }

  public void Dispose()
  {
    foreach (var storage in storages)
    {
      if (storage.Value is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    nodes.Clear();
    storages.Clear();
  }

  private sealed record Node<T>(T Data)
  {
    public ActorStatus   Status   { get; set; }
    public Node<T>?      Parent   { get; set; }
    public List<Node<T>> Children { get; } = new();
  }
}
