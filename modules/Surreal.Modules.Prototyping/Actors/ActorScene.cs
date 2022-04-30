using Surreal.Timing;

namespace Surreal.Actors;

/// <summary>A scene of managed <see cref="Actor"/>s.</summary>
public sealed class ActorScene : IEnumerable<Actor>, IActorContext, IDisposable
{
  private readonly Dictionary<ActorId, Node<Actor>> nodes = new();
  private readonly Queue<Actor> spawnQueue = new();
  private readonly Queue<Actor> destroyQueue = new();

  private ulong nextActorId = 0;

  public ActorScene(IServiceProvider? services = null)
  {
    Services = services;
  }

  public IServiceProvider? Services { get; }

  public T Spawn<T>(T actor)
    where T : Actor
  {
    if (nodes.TryGetValue(actor.Id, out var node))
    {
      node.Status = ActorStatus.Active;
    }
    else
    {
      spawnQueue.Enqueue(actor);
    }

    return actor;
  }

  public void BeginFrame(TimeDelta deltaTime)
  {
    ProcessSpawnQueue();

    foreach (var actor in this)
    {
      actor.OnBeginFrame(deltaTime);
    }
  }

  public void Input(TimeDelta deltaTime)
  {
    foreach (var actor in this)
    {
      actor.OnInput(deltaTime);
    }
  }

  public void Update(TimeDelta deltaTime)
  {
    foreach (var actor in this)
    {
      actor.OnUpdate(deltaTime);
    }
  }

  public void Draw(TimeDelta deltaTime)
  {
    foreach (var actor in this)
    {
      actor.OnDraw(deltaTime);
    }
  }

  public void EndFrame(TimeDelta deltaTime)
  {
    foreach (var actor in this)
    {
      actor.OnEndFrame(deltaTime);
    }

    ProcessDestroyQueue();
  }

  public void Clear()
  {
    // clears the scene of nodes
    foreach (var node in nodes.Values)
    {
      node.Data.Destroy();
    }

    ProcessDestroyQueue();
  }

  private void ProcessSpawnQueue()
  {
    while (spawnQueue.TryDequeue(out var actor))
    {
      actor.Connect(this);

      nodes[actor.Id] = new Node<Actor>(actor)
      {
        Status = ActorStatus.Active
      };

      // TODO: split these up, better FSM over actors
      actor.OnEnable();
      actor.OnAwake();
      actor.OnStart();
    }
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
    Clear();
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  IEnumerator<Actor> IEnumerable<Actor>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  /// <summary>A single node in the scene.</summary>
  private sealed record Node<T>(T Data)
  {
    public ActorStatus Status { get; set; }
  }

  /// <summary>Allows enumerating active <see cref="Actor"/>s.</summary>
  public struct Enumerator : IEnumerator<Actor>
  {
    private readonly ActorScene scene;
    private Dictionary<ActorId, Node<Actor>>.ValueCollection.Enumerator enumerator;

    public Enumerator(ActorScene scene)
      : this()
    {
      this.scene = scene;
      Reset();
    }

    public Actor       Current => enumerator.Current.Data;
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      while (enumerator.MoveNext())
      {
        if (enumerator.Current.Status == ActorStatus.Active)
        {
          return true;
        }
      }

      return false;
    }

    public void Reset()
    {
      enumerator = scene.nodes.Values.GetEnumerator();
    }

    public void Dispose()
    {
      // no-op
    }
  }
}
