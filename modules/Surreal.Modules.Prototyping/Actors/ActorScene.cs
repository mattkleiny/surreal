using Surreal.Timing;

namespace Surreal.Actors;

/// <summary>A scene of managed <see cref="Actor"/>s.</summary>
public sealed class ActorScene : IActorContext, IDisposable
{
  private readonly Dictionary<ActorId, Node<Actor>> nodes = new();
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

  public void BeginFrame(TimeDelta deltaTime)
  {
    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnBeginFrame(deltaTime);
      }
    }
  }

  public void Input(TimeDelta deltaTime)
  {
    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnInput(deltaTime);
      }
    }
  }

  public void Update(TimeDelta deltaTime)
  {
    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnUpdate(deltaTime);
      }
    }
  }

  public void Draw(TimeDelta deltaTime)
  {
    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnDraw(deltaTime);
      }
    }
  }

  public void EndFrame(TimeDelta deltaTime)
  {
    foreach (var node in nodes.Values)
    {
      if (node.Status == ActorStatus.Active)
      {
        node.Data.OnEndFrame(deltaTime);
      }
    }

    ProcessDestroyQueue();
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
    nodes.Clear();
  }

  /// <summary>A single node in the scene.</summary>
  private sealed record Node<T>(T Data)
  {
    public ActorStatus Status { get; set; }
  }
}
