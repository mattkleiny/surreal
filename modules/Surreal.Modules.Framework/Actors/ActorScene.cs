using System.Runtime.CompilerServices;
using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Actors;

/// <summary>A scene of managed <see cref="Actor" />s.</summary>
public sealed class ActorScene : IEnumerable<Actor>, IActorContext, IDisposable
{
  private readonly Queue<ArenaIndex> _destroyQueue = new();
  private readonly GenerationalArena<Node<Actor>> _nodes = new();
  private readonly Queue<ArenaIndex> _spawnQueue = new();

  public ActorScene(IServiceProvider? services = null)
  {
    Services = services;
  }

  public IServiceProvider? Services { get; }

  public T Spawn<T>(T actor)
    where T : Actor
  {
    _spawnQueue.Enqueue(_nodes.Add(new Node<Actor>(actor)));

    return actor;
  }

  public ActorStatus GetStatus(ArenaIndex index)
  {
    ref var node = ref _nodes[index];

    if (Unsafe.IsNullRef(ref node))
    {
      return ActorStatus.Unknown;
    }

    return node.Status;
  }

  void IActorContext.Enable(ArenaIndex index)
  {
    ref var node = ref _nodes[index];

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
    ref var node = ref _nodes[index];

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
    ref var node = ref _nodes[index];

    if (Unsafe.IsNullRef(ref node))
    {
      return;
    }

    if (node.Status != ActorStatus.Destroyed)
    {
      node.Status = ActorStatus.Destroyed;

      _destroyQueue.Enqueue(index);
    }
  }

  public void Dispose()
  {
    Clear();
  }

  IEnumerator<Actor> IEnumerable<Actor>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
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

    foreach (var actor in this) actor.OnBeginFrame(deltaTime);
  }

  public void Input(TimeDelta deltaTime)
  {
    foreach (var actor in this) actor.OnInput(deltaTime);
  }

  public void Update(TimeDelta deltaTime)
  {
    foreach (var actor in this) actor.OnUpdate(deltaTime);
  }

  public void Draw(TimeDelta deltaTime)
  {
    foreach (var actor in this) actor.OnDraw(deltaTime);
  }

  public void EndFrame(TimeDelta deltaTime)
  {
    foreach (var actor in this) actor.OnEndFrame(deltaTime);

    ProcessDestroyQueue();
  }

  public void Clear()
  {
    // clears the scene of nodes
    foreach (var node in _nodes) node.Data.Destroy();

    ProcessDestroyQueue();
  }

  private void ProcessSpawnQueue()
  {
    if (_spawnQueue.Count <= 0)
    {
      return;
    }

    while (_spawnQueue.TryDequeue(out var id))
    {
      ref var node = ref _nodes[id];

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
    if (_destroyQueue.Count <= 0)
    {
      return;
    }

    while (_destroyQueue.TryDequeue(out var id))
    {
      ref var node = ref _nodes[id];

      if (Unsafe.IsNullRef(ref node))
      {
        continue;
      }

      var actor = _nodes[id].Data;

      node.Status = ActorStatus.Destroyed;

      // TODO: split these up, better FSM over internal actor states?
      actor.OnDisable();
      actor.OnDestroy();

      actor.Disconnect(this);

      _nodes.Remove(id);
    }
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this, ActorStatus.Active);
  }

  /// <summary>A single node in the scene.</summary>
  private record struct Node<T>(T Data)
  {
    public ActorStatus Status = ActorStatus.Unknown;
  }

  /// <summary>Allows enumerating active <see cref="Actor" />s.</summary>
  public struct Enumerator : IEnumerator<Actor>
  {
    private readonly ActorScene _scene;
    private readonly ActorStatus _status;
    private GenerationalArena<Node<Actor>>.Enumerator _enumerator;

    public Enumerator(ActorScene scene, ActorStatus status)
      : this()
    {
      _scene = scene;
      _status = status;

      Reset();
    }

    public Actor Current => _enumerator.Current.Data;
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      while (_enumerator.MoveNext())
        if (_enumerator.Current.Status == _status)
        {
          return true;
        }

      return false;
    }

    public void Reset()
    {
      _enumerator = _scene._nodes.GetEnumerator();
    }

    public void Dispose()
    {
      // no-op
    }
  }
}





