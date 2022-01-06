using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Surreal.Components;
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

  public static ActorId Invalid    => default;
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
  private IActorContext?        context;
  private InternalStorageGroup? internalStorageGroup = new();

  public   ActorId     Id     { get; } = ActorId.Allocate();
  internal ActorStatus Status => context?.GetStatus(Id) ?? ActorStatus.Unknown;

  public bool IsDestroyed => Status == ActorStatus.Destroyed;
  public bool IsActive    => Status == ActorStatus.Active;
  public bool IsInactive  => Status == ActorStatus.Inactive;

  public void Enable()  => context?.Enable(Id);
  public void Disable() => context?.Disable(Id);
  public void Destroy() => context?.Destroy(Id);

  internal void ConnectToContext(IActorContext context, IComponentStorageGroup storageGroup)
  {
    // transfer components from the internal storage group to the context
    internalStorageGroup!.UnsafeTransferComponents(Id, storageGroup);
    internalStorageGroup = null;

    this.context = context;
  }

  internal void DisconnectFromContext(IActorContext context, IComponentStorageGroup storageGroup)
  {
    // transfer components to the local storage group
    internalStorageGroup = new InternalStorageGroup();
    storageGroup.UnsafeTransferComponents(Id, internalStorageGroup);

    this.context = null;
  }

  public ref T GetOrCreateComponent<T>(T prototype)
    where T : notnull
  {
    var storage = GetComponentStorage<T>();

    return ref storage.GetOrCreateComponent(Id, prototype);
  }

  public ref T AddComponent<T>(T prototype)
    where T : notnull
  {
    var storage = GetComponentStorage<T>();

    return ref storage.AddComponent(Id, prototype);
  }

  public ref T GetComponent<T>()
    where T : notnull
  {
    var     storage   = GetComponentStorage<T>();
    ref var component = ref storage.GetComponent(Id);

    if (Unsafe.IsNullRef(ref component))
    {
      throw new ActorException($"The given component is not available on the actor {typeof(T).Name}");
    }

    return ref component;
  }

  public bool RemoveComponent<T>()
    where T : notnull
  {
    var storage = GetComponentStorage<T>();

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

  private IComponentStorage<T> GetComponentStorage<T>()
    where T : notnull
  {
    if (context == null)
    {
      if (internalStorageGroup == null)
      {
        throw new InvalidOperationException("Missing internal storage in unlinked actor");
      }

      return internalStorageGroup.GetOrCreateStorage<T>();
    }

    return context.GetStorage<T>();
  }

  /// <summary>An internal <see cref="IComponentStorageGroup"/> for use in unlinked actors.</summary>
  private sealed class InternalStorageGroup : IComponentStorageGroup
  {
    private readonly Dictionary<Type, IComponentStorage> storagesByType = new();

    public IComponentStorage<T> GetOrCreateStorage<T>()
      where T : notnull
    {
      var type = typeof(T);

      if (!storagesByType.TryGetValue(type, out var storage))
      {
        storagesByType[type] = storage = new InternalStorage<T>();
      }

      return (IComponentStorage<T>) storage;
    }

    public IComponentStorage UnsafeGetOrCreateStorage(Type type)
    {
      if (!storagesByType.TryGetValue(type, out var storage))
      {
        var genericType = typeof(InternalStorage<>)
          .MakeGenericType(type);

        storagesByType[type] = storage = (IComponentStorage) Activator.CreateInstance(genericType)!;
      }

      return storage;
    }

    public void UnsafeTransferComponents(ActorId id, IComponentStorageGroup otherGroup)
    {
      foreach (var (componentType, storage) in storagesByType)
      {
        if (storage.UnsafeTryGetComponent(id, out var component))
        {
          var otherStorage = otherGroup.UnsafeGetOrCreateStorage(componentType);

          otherStorage.UnsafeAddComponent(id, component);
        }
      }
    }

    /// <summary>A <see cref="IComponentStorage{T}"/> for a single component <see cref="T"/>.</summary>
    private sealed class InternalStorage<T> : IComponentStorage<T>
      where T : notnull
    {
      private T    value   = default!;
      private bool isValid = false;

      public ref T GetOrCreateComponent(ActorId id, Optional<T> prototype)
      {
        if (!isValid)
        {
          value   = prototype.GetOrDefault(default!);
          isValid = true;
        }

        return ref value;
      }

      public ref T GetComponent(ActorId id)
      {
        if (!isValid)
        {
          return ref Unsafe.NullRef<T>();
        }

        return ref value;
      }

      public ref T AddComponent(ActorId id, Optional<T> prototype)
      {
        value   = prototype.GetOrDefault(default!);
        isValid = true;
        return ref value;
      }

      public bool RemoveComponent(ActorId id)
      {
        value   = default!;
        isValid = false;

        return true;
      }

      public bool UnsafeTryGetComponent(ActorId id, [NotNullWhen(true)] out object? component)
      {
        component = value;
        return isValid;
      }

      public void UnsafeAddComponent(ActorId id, object component)
      {
        value   = (T) component;
        isValid = true;
      }
    }
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
