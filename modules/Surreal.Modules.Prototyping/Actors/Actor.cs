using System.Globalization;
using Surreal.Timing;

namespace Surreal.Actors;

/// <summary>The possible states for an <see cref="Actor"/>.</summary>
public enum ActorStatus
{
  Unknown,
  Active,
  Inactive,
  Destroyed
}

/// <summary>Uniquely identifies a single <see cref="Actor"/>.</summary>
public readonly record struct ActorId(ulong Id)
{
  public static ActorId Invalid => default;

  public bool IsInvalid => Id == 0;
  public bool IsValid   => Id != 0;

  public override string ToString()
  {
    return Id.ToString(CultureInfo.InvariantCulture);
  }
}

/// <summary>An actor in the game world.</summary>
/// <remarks>
/// This is a hybrid Game Object/ECS model that permits fast internal iteration of ECS-bound
/// components as well as high-level composition of game logic via classes and inheritance.
/// </remarks>
public class Actor
{
  private IActorContext? context;

  public Actor()
  {
    Behaviours = new ActorBehaviourList(this);
  }

  public ActorId Id { get; private set; } = ActorId.Invalid;

  public ActorStatus       Status   => context?.GetStatus(Id) ?? ActorStatus.Unknown;
  public IServiceProvider? Services => context?.Services;

  public bool IsDestroyed => Status == ActorStatus.Destroyed;
  public bool IsActive    => Status == ActorStatus.Active;
  public bool IsInactive  => Status == ActorStatus.Inactive;

  public ActorBehaviourList Behaviours { get; }

  public void Enable() => context?.Enable(Id);
  public void Disable() => context?.Disable(Id);
  public void Destroy() => context?.Destroy(Id);

  public Actor AddBehaviour(ActorBehaviour behaviour)
  {
    Behaviours.Add(behaviour);

    return this;
  }

  internal void Connect(IActorContext context)
  {
    this.context = context;

    Id = context.AllocateId();
  }

  internal void Disconnect(IActorContext context)
  {
    this.context = null;

    Id = ActorId.Invalid;
  }

  public bool TryGetBehaviour<T>(out T result)
    where T : ActorBehaviour
  {
    return Behaviours.TryGet(out result);
  }

  protected internal virtual void OnAwake()
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnAwake();
    }
  }

  protected internal virtual void OnStart()
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnStart();
    }
  }

  protected internal virtual void OnEnable()
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnEnable();
    }
  }

  protected internal virtual void OnBeginFrame(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnBeginFrame(deltaTime);
    }
  }

  protected internal virtual void OnInput(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnInput(deltaTime);
    }
  }

  protected internal virtual void OnUpdate(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnUpdate(deltaTime);
    }
  }

  protected internal virtual void OnDraw(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnDraw(deltaTime);
    }
  }

  protected internal virtual void OnEndFrame(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnEndFrame(deltaTime);
    }
  }

  protected internal virtual void OnDisable()
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnDisable();
    }
  }

  protected internal virtual void OnDestroy()
  {
    foreach (var behaviour in Behaviours)
    {
      behaviour.OnDestroy();
    }
  }
}
