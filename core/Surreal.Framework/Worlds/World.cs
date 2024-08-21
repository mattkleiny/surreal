using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Worlds;

public readonly record struct TickEvent(DeltaTime DeltaTime);
public readonly record struct RenderEvent(DeltaTime DeltaTime);

public interface IEntityQuery
{
}

public interface IEntitySystem
{
  bool CanHandle<TEvent>();
  void OnEvent<TEvent>(TEvent @event, IEntityQuery query);
}

public sealed class World : IEntityQuery
{
  private readonly Arena<Entity> _entities = new();
  private readonly MultiDictionary<Type, IEntitySystem> _systems = new();

  public EntityHandle Spawn()
  {
    var index = _entities.Add(new Entity());

    return EntityHandle.FromArenaIndex(index);
  }

  public void Despawn(EntityHandle handle)
  {
    _entities.Remove(handle);
  }

  public void AddComponent<TComponent>(EntityHandle handle, TComponent component)
  {
    throw new NotImplementedException();
  }

  public void RemoveComponent<TComponent>(EntityHandle handle, TComponent component)
  {
    throw new NotImplementedException();
  }

  public ref TComponent GetComponent<TComponent>(EntityHandle handle)
  {
    throw new NotImplementedException();
  }

  public void AddSystem<TEvent>(IEntitySystem system)
  {
    _systems.Add(typeof(TEvent), system);
  }

  public void RemoveSystem<TEvent>(IEntitySystem system)
  {
    _systems.Remove(typeof(TEvent), system);
  }

  public void Publish<TEvent>(TEvent @event)
  {
    foreach (var system in _systems[typeof(TEvent)])
    {
      system.OnEvent(@event, this);
    }
  }

  /// <summary>
  /// A single entity in the world.
  /// </summary>
  private sealed class Entity
  {
    // TODO: component storage
  }
}

public static class WorldExtensions
{
  public static IDisposable AddSystem<TEvent>(this World world, Delegate @delegate)
  {
    var system = AnonymousSystem<TEvent>.Create(@delegate);

    world.AddSystem<TEvent>(system);

    return Disposables.Anonymous(() =>
    {
      world.RemoveSystem<TEvent>(system);
    });
  }

  private sealed class AnonymousSystem<TTarget>(Delegate @delegate) : IEntitySystem
  {
    public static AnonymousSystem<TTarget> Create(Delegate @delegate)
    {
      // TODO: double check that the delegate is a valid system
      // TODO: build a compiled delegate for executing the system at runtime

      return new AnonymousSystem<TTarget>(@delegate);
    }

    public bool CanHandle<TEvent>()
    {
      return typeof(TEvent) == typeof(TTarget);
    }

    public void OnEvent<TEvent>(TEvent @event, IEntityQuery query)
    {
      throw new NotImplementedException();
    }
  }
}

/// <summary>
/// An opaque handle to an entity in a scene.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct EntityHandle(ulong Id) : IDisposable
{
  public static EntityHandle None => default;

  public static EntityHandle FromInt(int index) => new((ulong)index);
  public static EntityHandle FromUInt(uint index) => new(index);
  public static EntityHandle FromNInt(nint index) => new((ulong)index);
  public static EntityHandle FromULong(ulong index) => new(index);
  public static EntityHandle FromArenaIndex(ArenaIndex index) => new(index);
  public static EntityHandle FromPointer(IntPtr pointer) => new((ulong)pointer);
  public static unsafe EntityHandle FromPointer(void* pointer) => new((ulong)pointer);

  /// <summary>
  /// Creates a new handle from the given object by pinning it in memory and taking a pointer to it
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static EntityHandle FromObject(object? value)
  {
    var handle = GCHandle.Alloc(value, GCHandleType.Pinned);
    var pointer = GCHandle.ToIntPtr(handle);

    return FromPointer(pointer);
  }

  public static implicit operator int(EntityHandle handle) => (int)handle.Id;
  public static implicit operator uint(EntityHandle handle) => (uint)handle.Id;
  public static implicit operator nint(EntityHandle handle) => (nint)handle.Id;
  public static implicit operator ulong(EntityHandle handle) => handle.Id;
  public static implicit operator ArenaIndex(EntityHandle handle) => ArenaIndex.FromUlong(handle);
  public static unsafe implicit operator void*(EntityHandle handle) => (void*)handle.Id;

  /// <summary>
  /// Converts the handle to an integer.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public unsafe void* AsPointer() => (void*)Id;

  /// <summary>
  /// Converts the handle to a pointer of type <typeparamref name="T" />.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public unsafe T* AsPointer<T>() where T : unmanaged => (T*)(void*)Id;

  /// <summary>
  /// Converts the handle to an object of type <typeparamref name="T" />.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public T AsObject<T>() where T : class
  {
    var handle = GCHandle.FromIntPtr(this);
    var target = handle.Target;

    if (target == null)
    {
      throw new NullReferenceException("The object pointed at by this handle is null.");
    }

    return (T)target;
  }

  /// <summary>
  /// Frees the GC handle of the value pointed at by this handle, assuming it's a pointer.
  /// </summary>
  public void Dispose()
  {
    var handle = GCHandle.FromIntPtr(this);
    if (handle.IsAllocated)
    {
      if (handle.Target is IDisposable disposable)
      {
        disposable.Dispose();
      }

      handle.Free();
    }
  }
}
