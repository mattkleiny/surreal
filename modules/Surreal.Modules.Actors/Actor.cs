using System.Globalization;
using System.Runtime.CompilerServices;
using Surreal.Collections;
using Surreal.Timing;

namespace Surreal;

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
  private static ulong nextId = 1;

  public static ActorId Invalid => default;
  public static ActorId Allocate() => new(Interlocked.Increment(ref nextId));

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
  private readonly IActorContext context;

  public Actor(IActorContext context)
  {
    this.context = context;
  }

  public ActorId     Id     { get; } = ActorId.Allocate();
  public ActorStatus Status => context.GetStatus(Id);

  public bool IsDestroyed => Status == ActorStatus.Destroyed;
  public bool IsActive    => Status == ActorStatus.Active;
  public bool IsInactive  => Status == ActorStatus.Inactive;

  public void Enable() => context.Enable(Id);
  public void Disable() => context.Disable(Id);
  public void Destroy() => context.Destroy(Id);

  public ref T GetOrCreateComponent<T>(T prototype)
    where T : notnull, new()
  {
    var storage = context.GetStorage<T>();

    return ref storage.GetOrCreateComponent(Id, prototype);
  }

  public ref T AddComponent<T>(T prototype)
    where T : notnull, new()
  {
    var storage = context.GetStorage<T>();

    return ref storage.AddComponent(Id, prototype);
  }

  public ref T GetComponent<T>()
    where T : notnull, new()
  {
    var storage = context.GetStorage<T>();
    ref var component = ref storage.GetComponent(Id);

    if (Unsafe.IsNullRef(ref component))
    {
      throw new ActorException($"The given component is not available on the actor {typeof(T).Name}");
    }

    return ref component;
  }

  public bool RemoveComponent<T>()
    where T : notnull, new()
  {
    var storage = context.GetStorage<T>();

    return storage.RemoveComponent(Id);
  }

  protected internal virtual void OnAwake()
  {
  }

  protected internal virtual void OnEnable()
  {
  }

  protected internal virtual void OnInput(DeltaTime time)
  {
  }

  protected internal virtual void OnUpdate(DeltaTime time)
  {
  }

  protected internal virtual void OnDraw(DeltaTime time)
  {
  }

  protected internal virtual void OnDisable()
  {
  }

  protected internal virtual void OnDestroy()
  {
  }
}

/// <summary>Indicates an error with the Actor/Component system.</summary>
public sealed class ActorException : Exception
{
  public ActorException(string message)
    : base(message)
  {
  }
}
