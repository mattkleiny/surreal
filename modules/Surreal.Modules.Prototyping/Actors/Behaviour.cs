using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Actors;

/// <summary>A list of <see cref="Behaviour"/>s with managed reference tracking.</summary>
public sealed class BehaviourList : ListDecorator<Behaviour>
{
  private readonly Actor actor;

  public BehaviourList(Actor actor)
  {
    this.actor = actor;
  }

  /// <summary>Tries to locate the first behaviour of the given type, <see cref="T"/>.</summary>
  public bool TryGet<T>(out T result)
    where T : Behaviour
  {
    foreach (var behaviour in Items)
    {
      if (behaviour is T candidate)
      {
        result = candidate;
        return true;
      }
    }

    result = default!;
    return false;
  }

  protected override void OnItemAdded(Behaviour item)
  {
    base.OnItemAdded(item);

    item.Connect(actor);
  }

  protected override void OnItemRemoved(Behaviour item)
  {
    base.OnItemRemoved(item);

    item.Disconnect(actor);
  }
}

/// <summary>A behaviour is an object-oriented component that you can attach to an <see cref="Actor"/>.</summary>
public abstract class Behaviour
{
  public Actor Actor { get; private set; } = null!;

  internal void Connect(Actor actor)
  {
    Actor = actor;

    if (actor.Status == ActorStatus.Active)
    {
      OnEnable();
      OnAwake();
      OnStart();
    }
  }

  internal void Disconnect(Actor actor)
  {
    if (actor.Status == ActorStatus.Active)
    {
      OnDisable();
      OnDestroy();
    }

    Actor = null!;
  }

  public bool TryGetBehaviour<T>(out T result)
    where T : Behaviour
  {
    return Actor.Behaviours.TryGet(out result);
  }

  public virtual void OnAwake()
  {
  }

  public virtual void OnStart()
  {
  }

  public virtual void OnEnable()
  {
  }

  public virtual void OnBeginFrame(TimeDelta deltaTime)
  {
  }

  public virtual void OnInput(TimeDelta deltaTime)
  {
  }

  public virtual void OnUpdate(TimeDelta deltaTime)
  {
  }

  public virtual void OnDraw(TimeDelta deltaTime)
  {
  }

  public virtual void OnEndFrame(TimeDelta deltaTime)
  {
  }

  public virtual void OnDisable()
  {
  }

  public virtual void OnDestroy()
  {
  }
}

/// <summary>A <see cref="Behaviour"/> that expects a parent actor type <see cref="TActor"/>.</summary>
public abstract class Behaviour<TActor> : Behaviour
  where TActor : Actor
{
  public new TActor Actor => (TActor) base.Actor;
}
