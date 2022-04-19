using System.Reflection;
using Surreal.Aspects;
using Surreal.Components;
using Surreal.Systems;
using Surreal.Timing;

namespace Surreal;

/// <summary>A scene of managed <see cref="Actor"/>s.</summary>
public sealed class ActorScene : IActorContext, IDisposable
{
  private readonly Dictionary<ActorId, Node<Actor>> nodes = new();
  private readonly ComponentStorageGroup components = new();
  private readonly LinkedList<ActorSystem> actorSystems = new();
  private readonly LinkedList<ComponentSystem> componentSystems = new();
  private readonly LinkedList<AspectSubscription> subscriptions = new();
  private readonly Queue<Actor> destroyQueue = new();

  private ulong nextActorId = 0;

  public ActorScene(IServiceProvider? services = null)
  {
    Services = services;
  }

  public IServiceProvider? Services { get; }

  public void AddSystem(ActorSystem system)
  {
    actorSystems.AddLast(system);
    system.OnAddedToScene(this);
  }

  public void AddSystem(ComponentSystem system)
  {
    componentSystems.AddLast(system);
    system.OnAddedToScene(this);
  }

  public void RemoveSystem(ActorSystem system)
  {
    if (actorSystems.Remove(system))
    {
      system.OnRemovedFromScene(this);
    }
  }

  public void RemoveSystem(ComponentSystem system)
  {
    if (componentSystems.Remove(system))
    {
      system.OnRemovedFromScene(this);
    }
  }

  public AspectSubscription SubscribeToAspect(Aspect aspect)
  {
    return new AspectSubscription(aspect, this);
  }

  public void RemoveSubscription(AspectSubscription subscription)
  {
    subscriptions.Remove(subscription);
  }

  public T Spawn<T>(T actor)
    where T : Actor
  {
    actor.Connect(this);

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

      // TODO: split these up, better FSM over actors
      actor.OnEnable();
      actor.OnAwake();
      actor.OnStart();
    }

    return actor;
  }

  public void BeginFrame(DeltaTime deltaTime)
  {
    foreach (var system in actorSystems)
    {
      system.OnBeginFrame(deltaTime);
    }

    foreach (var system in componentSystems)
    {
      system.OnBeginFrame(deltaTime);
    }

    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnBeginFrame(deltaTime);
      }
    }
  }

  public void Input(DeltaTime deltaTime)
  {
    foreach (var system in actorSystems)
    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        system.OnInput(deltaTime, node.Data);
      }
    }

    foreach (var system in componentSystems)
    {
      system.OnInput(deltaTime);
    }

    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnInput(deltaTime);
      }
    }
  }

  public void Update(DeltaTime deltaTime)
  {
    foreach (var system in actorSystems)
    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        system.OnUpdate(deltaTime, node.Data);
      }
    }

    foreach (var system in componentSystems)
    {
      system.OnUpdate(deltaTime);
    }

    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnUpdate(deltaTime);
      }
    }
  }

  public void Draw(DeltaTime deltaTime)
  {
    foreach (var system in actorSystems)
    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        system.OnDraw(deltaTime, node.Data);
      }
    }

    foreach (var system in componentSystems)
    {
      system.OnDraw(deltaTime);
    }

    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnDraw(deltaTime);
      }
    }
  }

  public void EndFrame(DeltaTime deltaTime)
  {
    foreach (var system in actorSystems)
    {
      system.OnEndFrame(deltaTime);
    }

    foreach (var system in componentSystems)
    {
      system.OnEndFrame(deltaTime);
    }

    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnEndFrame(deltaTime);
      }
    }

    ProcessDestroyQueue();
    ProcessSubscriptions();
  }

  private void ProcessDestroyQueue()
  {
    while (destroyQueue.TryDequeue(out var actor))
    {
      // TODO: split these up, better FSM over internal actor states?
      actor.OnDisable();
      actor.OnDestroy();

      actor.Disconnect(this);

      nodes.Remove(actor.Id);
      components.RemoveAll(actor.Id);
    }
  }

  private void ProcessSubscriptions()
  {
    // TODO: make this work
    foreach (var subscription in subscriptions)
    {
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

  public IComponentStorage<T> GetStorage<T>()
    where T : notnull, new()
  {
    return components.GetOrCreateStorage<T>();
  }

  ActorId IActorContext.AllocateId()
  {
    return new ActorId(Interlocked.Increment(ref nextActorId));
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

  public void Dispose()
  {
    foreach (var system in actorSystems)
    {
      system.Dispose();
    }

    foreach (var system in componentSystems)
    {
      system.Dispose();
    }

    actorSystems.Clear();
    componentSystems.Clear();
    components.Dispose();
    nodes.Clear();
  }

  /// <summary>A single node in the scene.</summary>
  private sealed record Node<T>(T Data)
  {
    public ActorStatus Status { get; set; }
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
