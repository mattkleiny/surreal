using Surreal.Collections;
using Surreal.Timing;

namespace Surreal;

/// <summary>A list of <see cref="ActorBehaviour" />s with managed reference tracking.</summary>
public sealed class ActorBehaviourList : ListDecorator<ActorBehaviour>
{
  private readonly Actor _actor;

  public ActorBehaviourList(Actor actor)
  {
    _actor = actor;
  }

  /// <summary>Tries to locate the first behaviour of the given type, <see cref="T" />.</summary>
  public bool TryGet<T>(out T result)
    where T : ActorBehaviour
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

  protected override void OnItemAdded(ActorBehaviour item)
  {
    base.OnItemAdded(item);

    item.Connect(_actor);
  }

  protected override void OnItemRemoved(ActorBehaviour item)
  {
    base.OnItemRemoved(item);

    item.Disconnect(_actor);
  }
}

/// <summary>A behaviour is an object-oriented component that you can attach to an <see cref="Actor" />.</summary>
public abstract class ActorBehaviour
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
    where T : ActorBehaviour
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


