using System.Reflection;
using Surreal.Aspects;
using Surreal.Components;
using Surreal.Systems;
using Surreal.Timing;

namespace Surreal;

/// <summary>A scene of managed <see cref="Actor"/>s.</summary>
public sealed class ActorScene : IActorContext, IComponentSystemContext, IDisposable
{
  private readonly Dictionary<ActorId, Node<Actor>> nodes        = new();
  private readonly LinkedList<IComponentSystem>     systems      = new();
  private readonly SceneStorageGroup                storageGroup = new();

  private readonly Queue<Actor> destroyQueue = new();

  public void AddSystem(IComponentSystem system)    => systems.AddLast(system);
  public void RemoveSystem(IComponentSystem system) => systems.Remove(system);

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

      actor.ConnectToContext(this, storageGroup);
      actor.OnAwake();
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

  private void ProcessDestroyQueue()
  {
    while (destroyQueue.TryDequeue(out var actor))
    {
      actor.DisconnectFromContext(this, storageGroup);
      actor.OnDestroy();

      nodes.Remove(actor.Id);
    }
  }

  public void Dispose()
  {
    storageGroup.Dispose();
    nodes.Clear();
  }

  IComponentStorage<T> IActorContext.GetStorage<T>()
  {
    return storageGroup.GetOrCreateStorage<T>();
  }

  AspectEnumerator IComponentSystemContext.QueryActors(Aspect aspect)
  {
    throw new NotImplementedException();
  }

  /// <summary>A single node in the scene.</summary>
  private sealed record Node<T>(T Data)
  {
    public ActorStatus   Status   { get; set; }
    public Node<T>?      Parent   { get; set; }
    public List<Node<T>> Children { get; } = new();
  }

  /// <summary>An <see cref="IComponentStorageGroup"/> for use in scene actors.</summary>
  private sealed class SceneStorageGroup : IComponentStorageGroup, IDisposable
  {
    private readonly Dictionary<Type, IComponentStorage> storagesByType = new();

    public IComponentStorage UnsafeGetOrCreateStorage(Type type)
    {
      if (!storagesByType.TryGetValue(type, out var storage))
      {
        var factoryMethod = typeof(SceneStorageGroup)
          .GetMethod(nameof(CreateStorage), BindingFlags.NonPublic | BindingFlags.Static)!
          .MakeGenericMethod(type);

        storagesByType[type] = storage = (IComponentStorage) factoryMethod.Invoke(null, null)!;
      }

      return storage;
    }

    public IComponentStorage<T> GetOrCreateStorage<T>()
      where T : notnull
    {
      if (!storagesByType.TryGetValue(typeof(T), out var storage))
      {
        storagesByType[typeof(T)] = storage = CreateStorage<T>();
      }

      return (IComponentStorage<T>) storage;
    }

    public void UnsafeTransferComponents(ActorId id, IComponentStorageGroup otherGroup)
    {
      throw new NotImplementedException();
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
      where T : notnull
    {
      var storageType = typeof(SparseComponentStorage<>);
      var attribute   = typeof(T).GetCustomAttribute<ComponentAttribute>();

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
