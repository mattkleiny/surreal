using System.Globalization;
using System.Runtime.CompilerServices;
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

  public ActorId     Id     { get; private set; } = ActorId.Invalid;
  public ActorStatus Status => context?.GetStatus(Id) ?? ActorStatus.Unknown;

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

  public ref T GetOrCreateComponent<T>()
    where T : notnull, new()
  {
    return ref GetOrCreateComponent(new T());
  }

  public ref T GetOrCreateComponent<T>(T prototype)
    where T : notnull, new()
  {
    if (context == null)
    {
      throw new ActorException("The actor is not currently part of a scene, it's component APIs are not available");
    }

    var storage = context.GetStorage<T>();

    return ref storage.GetOrCreateComponent(Id, prototype);
  }

  public ref T AddComponent<T>(T prototype)
    where T : notnull, new()
  {
    if (context == null)
    {
      throw new ActorException("The actor is not currently part of a scene, it's component APIs are not available");
    }

    var storage = context.GetStorage<T>();

    return ref storage.AddComponent(Id, prototype);
  }

  public ref T GetComponent<T>()
    where T : notnull, new()
  {
    if (context == null)
    {
      throw new ActorException("The actor is not currently part of a scene, it's component APIs are not available");
    }

    var storage = context.GetStorage<T>();
    ref var component = ref storage.GetComponent(Id);

    if (Unsafe.IsNullRef(ref component))
    {
      throw new ActorException($"The given component '{typeof(T).Name}' is not available on actor id {Id}");
    }

    return ref component;
  }

  public bool RemoveComponent<T>()
    where T : notnull, new()
  {
    if (context == null)
    {
      throw new ActorException("The actor is not currently part of a scene, it's component APIs are not available");
    }

    var storage = context.GetStorage<T>();

    return storage.RemoveComponent(Id);
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

  protected internal virtual void OnInput(DeltaTime time)
  {
  }

  protected internal virtual void OnUpdate(DeltaTime deltaTime)
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
