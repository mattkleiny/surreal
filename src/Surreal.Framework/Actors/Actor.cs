﻿using System.Runtime.CompilerServices;

namespace Surreal.Actors;

public enum ActorStatus
{
  Unknown,
  Active,
  Inactive,
  Destroyed
}

public class Actor
{
  private IActorContext context = null!;

  public ActorId     Id     { get; } = ActorId.Allocate();
  public ActorStatus Status => context.GetStatus(Id);

  public bool IsDestroyed => Status == ActorStatus.Destroyed;
  public bool IsActive    => Status == ActorStatus.Active;
  public bool IsInactive  => Status == ActorStatus.Inactive;

  public void Enable()  => context.Enable(Id);
  public void Disable() => context.Disable(Id);

  internal void Awake(IActorContext context)
  {
    this.context = context;

    OnAwake();
  }

  public void Destroy()
  {
    if (!IsDestroyed)
    {
      context.Destroy(Id);
    }
  }

  #region Components

  public T AddComponent<T>(T prototype)
  {
    var storage = context.GetStorage<T>();

    return storage.AddComponent(Id, prototype);
  }

  public ref T GetComponent<T>()
  {
    var     storage   = context.GetStorage<T>();
    ref var component = ref storage.GetComponent(Id);

    if (Unsafe.IsNullRef(ref component))
    {
      throw new Exception($"The given component is not available on the actor {typeof(T).Name}");
    }

    return ref component!;
  }

  public bool RemoveComponent<T>()
  {
    var storage = context.GetStorage<T>();

    return storage.RemoveComponent(Id);
  }

  #endregion

  #region Callbacks

  protected internal virtual void OnAwake()
  {
  }

  protected internal virtual void OnEnable()
  {
  }

  protected internal virtual void OnInput(GameTime time)
  {
  }

  protected internal virtual void OnUpdate(GameTime time)
  {
  }

  protected internal virtual void OnDraw(GameTime time)
  {
  }

  protected internal virtual void OnDisable()
  {
  }

  protected internal virtual void OnDestroy()
  {
  }

  #endregion
}
