using System.Runtime.CompilerServices;
using Surreal.Collections;
using Surreal.Timing;

namespace Surreal;

/// <summary>A scene of managed <see cref="Actor"/>s.</summary>
public sealed class ActorScene : IEnumerable<Actor>, IActorContext, IDisposable
{
  private readonly GenerationalArena<Node<Actor>> nodes = new();
  private readonly Queue<ArenaIndex> spawnQueue = new();
  private readonly Queue<ArenaIndex> destroyQueue = new();

  public ActorScene(IServiceProvider? services = null)
  {
    Services = services;
  }

  public IServiceProvider? Services { get; }

  public T Spawn<T>(T actor)
    where T : Actor
  {
    spawnQueue.Enqueue(nodes.Add(new Node<Actor>(actor)));

    return actor;
  }

  /// <summary>Ticks the entire scene in sequence.</summary>
  public void Tick(TimeDelta deltaTime)
  {
    BeginFrame(deltaTime);
    Input(deltaTime);
    Draw(deltaTime);
    Update(deltaTime);
    EndFrame(deltaTime);
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
    foreach (var node in nodes)
    {
      node.Data.Destroy();
    }

    ProcessDestroyQueue();
  }

  private void ProcessSpawnQueue()
  {
    if (spawnQueue.Count <= 0) return;

    while (spawnQueue.TryDequeue(out var id))
    {
      ref var node = ref nodes[id];

      if (Unsafe.IsNullRef(ref node))
      {
        continue;
      }

      var actor = node.Data;

      node.Status = ActorStatus.Active;

      actor.Connect(this, id);

      // TODO: split these up, better FSM over actors?
      actor.OnEnable();
      actor.OnAwake();
      actor.OnStart();
    }
  }

  private void ProcessDestroyQueue()
  {
    if (destroyQueue.Count <= 0) return;

    while (destroyQueue.TryDequeue(out var id))
    {
      ref var node = ref nodes[id];

      if (Unsafe.IsNullRef(ref node))
      {
        continue;
      }

      var actor = nodes[id].Data;

      node.Status = ActorStatus.Destroyed;

      // TODO: split these up, better FSM over internal actor states?
      actor.OnDisable();
      actor.OnDestroy();

      actor.Disconnect(this);

      nodes.Remove(id);
    }
  }

  public ActorStatus GetStatus(ArenaIndex index)
  {
    ref var node = ref nodes[index];

    if (Unsafe.IsNullRef(ref node))
    {
      return ActorStatus.Unknown;
    }

    return node.Status;
  }

  void IActorContext.Enable(ArenaIndex index)
  {
    ref var node = ref nodes[index];

    if (Unsafe.IsNullRef(ref node))
    {
      return;
    }

    if (node.Status != ActorStatus.Destroyed)
    {
      node.Status = ActorStatus.Active;
    }
  }

  void IActorContext.Disable(ArenaIndex index)
  {
    ref var node = ref nodes[index];

    if (Unsafe.IsNullRef(ref node))
    {
      return;
    }

    if (node.Status != ActorStatus.Destroyed)
    {
      node.Status = ActorStatus.Inactive;
    }
  }

  void IActorContext.Destroy(ArenaIndex index)
  {
    ref var node = ref nodes[index];

    if (Unsafe.IsNullRef(ref node))
    {
      return;
    }

    if (node.Status != ActorStatus.Destroyed)
    {
      node.Status = ActorStatus.Destroyed;

      destroyQueue.Enqueue(index);
    }
  }

  public void Dispose()
  {
    Clear();
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this, ActorStatus.Active);
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
  private record struct Node<T>(T Data)
  {
    public ActorStatus Status = ActorStatus.Unknown;
  }

  /// <summary>Allows enumerating active <see cref="Actor"/>s.</summary>
  public struct Enumerator : IEnumerator<Actor>
  {
    private readonly ActorScene scene;
    private readonly ActorStatus status;
    private GenerationalArena<Node<Actor>>.Enumerator enumerator;

    public Enumerator(ActorScene scene, ActorStatus status)
      : this()
    {
      this.scene  = scene;
      this.status = status;

      Reset();
    }

    public Actor       Current => enumerator.Current.Data;
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      while (enumerator.MoveNext())
      {
        if (enumerator.Current.Status == status)
        {
          return true;
        }
      }

      return false;
    }

    public void Reset()
    {
      enumerator = scene.nodes.GetEnumerator();
    }

    public void Dispose()
    {
      // no-op
    }
  }
}
