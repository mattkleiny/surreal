using System.Globalization;
using Surreal.Timing;

namespace Surreal.Actors;

/// <summary>The possible states for an <see cref="Actor"/>.</summary>
public enum ActorStatus
{
  Unknown,
  Active,
  Inactive,
  Destroyed,
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

  public ActorId Id { get; private set; } = ActorId.Invalid;

  public ActorStatus       Status   => context?.GetStatus(Id) ?? ActorStatus.Unknown;
  public IServiceProvider? Services => context?.Services;

  public bool IsDestroyed => Status == ActorStatus.Destroyed;
  public bool IsActive    => Status == ActorStatus.Active;
  public bool IsInactive  => Status == ActorStatus.Inactive;

  public void Enable() => context?.Enable(Id);
  public void Disable() => context?.Disable(Id);
  public void Destroy() => context?.Destroy(Id);

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

  protected internal virtual void OnAwake()
  {
  }

  protected internal virtual void OnStart()
  {
  }

  protected internal virtual void OnEnable()
  {
  }

  protected internal virtual void OnBeginFrame(TimeDelta deltaTime)
  {
  }

  protected internal virtual void OnInput(TimeDelta deltaTime)
  {
  }

  protected internal virtual void OnUpdate(TimeDelta deltaTime)
  {
  }

  protected internal virtual void OnDraw(TimeDelta time)
  {
  }

  protected internal virtual void OnEndFrame(TimeDelta deltaTime)
  {
  }

  protected internal virtual void OnDisable()
  {
  }

  protected internal virtual void OnDestroy()
  {
  }
}
