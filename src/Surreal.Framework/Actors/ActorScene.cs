using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Surreal.Framework.Actors.Components;

namespace Surreal.Framework.Actors
{
  public sealed class ActorScene : IActorContext, IComponentContext, IDisposable
  {
    private readonly Dictionary<ActorId, Entry>          entriesById      = new();
    private readonly Dictionary<Type, IComponentStorage> componentsByType = new();
    private readonly Queue<ActorId>                      activationQueue  = new();
    private readonly Queue<ActorId>                      destructionQueue = new();

    private uint nextActorId;

    public List<IComponentSystem> Systems { get; } = new();

    public void Spawn(Actor actor)
    {
      var id = actor.Id;

      if (!entriesById.TryGetValue(id, out _))
      {
        entriesById[id] = new Entry(actor);
        activationQueue.Enqueue(id);

        actor.Awake(this);
      }
    }

    public void Input(GameTime time)
    {
      foreach (var entry in entriesById.Values)
      {
        if (entry.Status == ActorStatus.Active)
        {
          entry.Actor.OnInput(time);
        }
      }
    }

    public void Update(GameTime time)
    {
      EnableActors();
      DestroyActors();

      foreach (var entry in entriesById.Values)
      {
        if (entry.Status == ActorStatus.Active)
        {
          entry.Actor.OnUpdate(time);
        }
      }
    }

    public void Draw(GameTime time)
    {
      foreach (var entry in entriesById.Values)
      {
        if (entry.Status == ActorStatus.Active)
        {
          entry.Actor.OnDraw(time);
        }
      }
    }

    public void Clear()
    {
      foreach (var entry in entriesById.Values)
      {
        if (entry.Actor is IDisposable disposable)
        {
          disposable.Dispose();
        }
      }

      entriesById.Clear();
      componentsByType.Clear();
      activationQueue.Clear();
      destructionQueue.Clear();
    }

    public void Dispose()
    {
      Clear();
    }

    private void EnableActors()
    {
      while (activationQueue.TryDequeue(out var id))
      {
        if (entriesById.TryGetValue(id, out var entry))
        {
          entry.Transition(ActorStatus.Active);
        }
      }
    }

    private void DestroyActors()
    {
      while (destructionQueue.TryDequeue(out var id))
      {
        if (entriesById.TryGetValue(id, out var entry))
        {
          entry.Transition(ActorStatus.Destroyed);

          foreach (var storage in componentsByType.Values)
          {
            storage.Prune(id);
          }
        }
      }
    }

    private IComponentStorage<T> GetOrCreateStorage<T>()
    {
      var key = typeof(T);

      if (!componentsByType.TryGetValue(key, out var storage))
      {
        var attribute = key.GetCustomAttribute<ComponentAttribute>();
        if (attribute != null)
        {
          componentsByType[key] = storage = attribute.CreateStorage<T>();
        }
        else
        {
          componentsByType[key] = storage = new SparseComponentStorage<T>();
        }
      }

      return (IComponentStorage<T>)storage;
    }

    ActorId IActorContext.AllocateId()
    {
      return new(Interlocked.Increment(ref nextActorId));
    }

    ActorStatus IActorContext.GetStatus(ActorId id)
    {
      if (entriesById.TryGetValue(id, out var entry))
      {
        return entry.Status;
      }

      return ActorStatus.Unknown;
    }

    void IActorContext.Enable(ActorId id)
    {
      if (entriesById.TryGetValue(id, out var entry))
      {
        entry.Transition(ActorStatus.Active);
      }
    }

    void IActorContext.Disable(ActorId id)
    {
      if (entriesById.TryGetValue(id, out var entry))
      {
        entry.Transition(ActorStatus.Inactive);
      }
    }

    void IActorContext.Destroy(ActorId id)
    {
      destructionQueue.Enqueue(id);
    }

    IComponentStorage<T> IActorContext.GetStorage<T>()
    {
      return GetOrCreateStorage<T>();
    }

    public IComponentStorage<T> GetStorage<T>()
    {
      return GetOrCreateStorage<T>();
    }

    AspectEnumerator IComponentContext.GetEnumerator(Aspect aspect)
    {
      throw new NotImplementedException();
    }

    private sealed record Entry(Actor Actor)
    {
      public ActorStatus Status { get; private set; } = ActorStatus.Unknown;

      public void Transition(ActorStatus status)
      {
        if (Status != status)
        {
          Status = status;

          switch (status)
          {
            case ActorStatus.Active:
              Actor.OnEnable();
              break;
            case ActorStatus.Inactive:
              Actor.OnDisable();
              break;
            case ActorStatus.Destroyed:
              Actor.OnDestroy();
              break;
          }
        }
      }
    }
  }
}
