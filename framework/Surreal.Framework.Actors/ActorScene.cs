﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Surreal.Framework.Components;

namespace Surreal.Framework
{
  public sealed class ActorScene : IActorContext
  {
    private readonly Dictionary<ActorId, Entry>          entriesById      = new();
    private readonly Dictionary<Type, IComponentStorage> componentsByType = new();
    private readonly Queue<ActorId>                      activationQueue  = new();
    private readonly Queue<ActorId>                      destructionQueue = new();

    private uint nextActorId;

    public void Spawn(Actor actor)
    {
      var id = actor.Id;

      if (!entriesById.TryGetValue(id, out _))
      {
        entriesById[id] = new Entry(actor);
        activationQueue.Enqueue(id);
        
        actor.OnAwake();
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
      var key = typeof(T);

      if (!componentsByType.TryGetValue(key, out var storage))
      {
        componentsByType[key] = storage = new SparseComponentStorage<T>();
      }

      return (IComponentStorage<T>) storage;
    }

    // TODO: implement other storage types?

    private interface IComponentStorage
    {
      void Prune(ActorId id);
    }

    private sealed record Entry(Actor Actor)
    {
      public ActorStatus Status { get; private set; } = ActorStatus.Unknown;

      public void Transition(ActorStatus status)
      {
        if (Status == status) return;
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

    private sealed class SparseComponentStorage<T> : IComponentStorage<T>, IComponentStorage
    {
      private readonly Dictionary<ActorId, Box<T>> slots = new();

      public ref T GetComponent(ActorId id)
      {
        if (slots.TryGetValue(id, out var slot))
        {
          return ref slot.Value;
        }

        return ref Unsafe.NullRef<T>();
      }

      public ref T AddComponent(ActorId id, Optional<T> prototype)
      {
        var component = prototype.GetOrDefault()!;
        var slot      = new Box<T>(component);

        slots[id] = slot;

        return ref slot.Value;
      }

      public bool RemoveComponent(ActorId id)
      {
        return slots.Remove(id);
      }

      public void Prune(ActorId id)
      {
        slots.Remove(id);
      }
    }
  }
}