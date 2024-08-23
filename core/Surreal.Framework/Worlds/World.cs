using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Worlds;

public readonly record struct TickEvent(DeltaTime DeltaTime);
public readonly record struct RenderEvent(DeltaTime DeltaTime);

/// <summary>
/// A query that can be used to filter entities in the world.
/// </summary>
public interface IEntityQuery
{
  internal IEnumerable<EntityHandle> EnumerateEntities(ComponentInfo?[] componentInfos);

  internal object? ResolveComponent(EntityHandle entity, Type type);
  internal object? ResolveService(Type type);
}

/// <summary>
/// Marker interface for <see cref="IComponent{T}"/>s.
/// </summary>
public interface IComponent;

/// <summary>
/// Represents a component that can be attached to an entity.
/// </summary>
public interface IComponent<TSelf> : IComponent
  where TSelf : IComponent<TSelf>
{
  /// <summary>
  /// Creates a new storage for this component. By default, this creates a <see cref="DenseComponentStorage{TSelf}"/>.
  /// </summary>
  static virtual IComponentStorage<TSelf> CreateStorage()
  {
    return new SparseComponentStorage<TSelf>();
  }
}

/// <summary>
/// Marker interface for <see cref="IComponentStorage{T}"/>.
/// </summary>
public interface IComponentStorage;

/// <summary>
/// Represents a storage for components of type <typeparamref name="T"/>.
/// </summary>
public interface IComponentStorage<T> : IComponentStorage
  where T : IComponent<T>
{
  ref T Get(EntityHandle handle);
  void Add(EntityHandle handle, T component);
  void Remove(EntityHandle handle, T component);
}

/// <summary>
/// A <see cref="IComponentStorage{T}"/> that uses a sparse array to store components.
/// </summary>
public class SparseComponentStorage<T> : IComponentStorage<T>
  where T : IComponent<T>
{
  private readonly Dictionary<EntityHandle, T> _components = new();

  public ref T Get(EntityHandle handle)
  {
    return ref CollectionsMarshal.GetValueRefOrNullRef(_components, handle);
  }

  public void Add(EntityHandle handle, T component)
  {
    _components.Add(handle, component);
  }

  public void Remove(EntityHandle handle, T component)
  {
    _components.Remove(handle);
  }
}

/// <summary>
/// A <see cref="IComponentStorage{T}"/> that uses a dense array to store components.
/// </summary>
public class DenseComponentStorage<T> : IComponentStorage<T>
  where T : IComponent<T>
{
  private readonly List<T> _components = new();

  public ref T Get(EntityHandle handle)
  {
    throw new NotImplementedException();
  }

  public void Add(EntityHandle handle, T component)
  {
    throw new NotImplementedException();
  }

  public void Remove(EntityHandle handle, T component)
  {
    throw new NotImplementedException();
  }
}

/// <summary>
/// Marker interface for entity systems.
/// </summary>
public interface IEntitySystem;

/// <summary>
/// A system that processes <see cref="TEvent"/>s for entities in the world.
/// </summary>
public interface IEntitySystem<in TEvent> : IEntitySystem
{
  void OnEvent(TEvent @event, IEntityQuery query);
}

/// <summary>
/// An ECS world that manages entities, components, and systems.
/// </summary>
public sealed class World(IServiceProvider services)
{
  private readonly Arena<Entity> _entities = new();
  private readonly Dictionary<Type, IComponentStorage> _storages = new();
  private readonly MultiDictionary<Type, IEntitySystem> _systems = new();

  public IEntityQuery Query => new EntityQuery(this, services);

  /// <summary>
  /// Spawns a new entity in the world.
  /// </summary>
  public EntityHandle Spawn()
  {
    var index = _entities.Add(new Entity());

    return EntityHandle.FromArenaIndex(index);
  }

  /// <summary>
  /// Despawns the entity with the given handle.
  /// </summary>
  public void Despawn(EntityHandle handle)
  {
    _entities.Remove(handle);
  }

  /// <summary>
  /// Gets the component of type <typeparamref name="T"/> for the entity.
  /// </summary>
  public ref T GetComponent<T>(EntityHandle handle)
    where T : IComponent<T>
  {
    if (!_entities.TryGetValue(handle, out var entity))
    {
      throw new ArgumentException($"The entity with handle {handle} does not exist.");
    }

    if (!entity.Components.Contains(typeof(T)))
    {
      throw new ArgumentException($"The entity with handle {handle} does not have a component of type {typeof(T)}.");
    }

    return ref GetOrCreateStorage<T>().Get(handle);
  }

  /// <summary>
  /// Adds a new component of type <typeparamref name="T"/> to the entity.
  /// </summary>
  public void AddComponent<T>(EntityHandle handle, T component)
    where T : IComponent<T>
  {
    if (!_entities.TryGetValue(handle, out var entity))
    {
      throw new ArgumentException($"The entity with handle {handle} does not exist.");
    }

    if (entity.Components.Add(typeof(T)))
    {
      GetOrCreateStorage<T>().Add(handle, component);
    }
  }

  /// <summary>
  /// Removes the given component from the entity.
  /// </summary>
  public void RemoveComponent<T>(EntityHandle handle, T component)
    where T : IComponent<T>
  {
    if (!_entities.TryGetValue(handle, out var entity))
    {
      throw new ArgumentException($"The entity with handle {handle} does not exist.");
    }

    if (entity.Components.Remove(typeof(T)))
    {
      GetOrCreateStorage<T>().Remove(handle, component);
    }
  }

  /// <summary>
  /// Adds a new system to the world.
  /// </summary>
  public void AddSystem<TEvent>(IEntitySystem<TEvent> system)
  {
    _systems.Add(typeof(TEvent), system);
  }

  /// <summary>
  /// Removes the given system from the world.
  /// </summary>
  public void RemoveSystem<TEvent>(IEntitySystem<TEvent> system)
  {
    _systems.Remove(typeof(TEvent), system);
  }

  /// <summary>
  /// Publishes the given event to all systems that can handle it.
  /// </summary>
  public void Publish<TEvent>(TEvent @event)
  {
    foreach (var system in _systems[typeof(TEvent)])
    {
      var typedSystem = (IEntitySystem<TEvent>)system;

      typedSystem.OnEvent(@event, Query);
    }
  }

  /// <summary>
  /// Gets or creates a storage for components of type <typeparamref name="T"/>.
  /// </summary>
  private IComponentStorage<T> GetOrCreateStorage<T>()
    where T : IComponent<T>
  {
    if (!_storages.TryGetValue(typeof(T), out var storage))
    {
      _storages[typeof(T)] = storage = T.CreateStorage();
    }

    return (IComponentStorage<T>)storage;
  }

  /// <summary>
  /// A single entity in the world.
  /// </summary>
  private sealed class Entity
  {
    public HashSet<Type> Components { get; } = new();
  }

  /// <summary>
  /// The <see cref="IEntityQuery"/> implementation.
  /// </summary>
  private sealed class EntityQuery(World world, IServiceProvider services) : IEntityQuery
  {
    public IEnumerable<EntityHandle> EnumerateEntities(ComponentInfo?[] componentInfos)
    {
      throw new NotImplementedException();
    }

    public object? ResolveComponent(EntityHandle entity, Type type)
    {
      throw new NotImplementedException();
    }

    public object? ResolveService(Type serviceType)
    {
      return services.GetService(serviceType);
    }

  }
}

/// <summary>
/// Helpers for working with <see cref="World"/> instances.
/// </summary>
public static class WorldExtensions
{
  /// <summary>
  /// Adds a new system to the world that using the given anonymous <see cref="Delegate"/>.
  /// <para/>
  /// The <see cref="IDisposable"/> returned can be used to remove the system from the world.
  /// </summary>
  public static IDisposable AddSystem<TEvent>(this World world, Delegate @delegate)
  {
    var system = AnonymousSystem.Create<TEvent>(@delegate);

    world.AddSystem(system);

    return Disposables.Anonymous(() =>
    {
      world.RemoveSystem(system);
    });
  }

  /// <summary>
  /// Static factory for creating anonymous systems.
  /// </summary>
  private static class AnonymousSystem
  {
    /// <summary>
    /// Creates a new anonymous system that wraps the given delegate.
    /// </summary>
    public static AnonymousSystem<TEvent> Create<TEvent>(Delegate callback)
    {
      // double check that the delegate is a valid system
      var methodInfo = callback.GetMethodInfo();
      var parameterInfos = methodInfo.GetParameters();
      var componentInfos = new ComponentInfo?[parameterInfos.Length];

      for (var i = 0; i < parameterInfos.Length; i++)
      {
        var parameterInfo = parameterInfos[i];
        var parameterType = parameterInfo.ParameterType;
        
        if (parameterType.IsByRef) 
        {
          parameterType = parameterType.GetElementType()!;
        }

        if (parameterType.IsAssignableTo(typeof(IComponent)))
        {
          componentInfos[i] = ComponentInfo.Create(parameterType);
        }
        else if (!parameterType.IsAssignableTo(typeof(TEvent)))
        {
          throw new ArgumentException($"The parameter type {parameterType} is not allowed in an anonymous system.");
        }
      }

      // make sure the return type is void
      if (!methodInfo.ReturnType.IsAssignableTo(typeof(void)))
      {
        throw new ArgumentException("The return type of an anonymous system must be void.");
      }

      // build and cache the parameter array for the delegate
      var parameters = new object?[parameterInfos.Length];

      return new AnonymousSystem<TEvent>((@event, query) =>
      {
        foreach (var entity in query.EnumerateEntities(componentInfos))
        {
          for (var i = 0; i < parameterInfos.Length; i++)
          {
            var parameterInfo = parameterInfos[i];
            var parameterType = parameterInfo.ParameterType;

            // otherwise, query for the component type
            if (componentInfos[i] is { Type: var type })
            {
              parameters[i] = query.ResolveComponent(entity, type);
            }
            else if (parameterType == typeof(TEvent))
            {
              // forward the event payload in
              parameters[i] = @event;
            }
            else 
            {
            // otherwise, query for the service type
              parameters[i] = query.ResolveService(parameterType);
            }
          }

          callback.DynamicInvoke(parameters);
        }
      });
    }
  }

  /// <summary>
  /// An anonymous <see cref="IEntitySystem{TEvent}"/> that wraps a delegate.
  /// </summary>
  private sealed class AnonymousSystem<TEvent>(Action<TEvent, IEntityQuery> action) : IEntitySystem<TEvent>
  {
    public void OnEvent(TEvent @event, IEntityQuery query)
    {
      action.Invoke(@event, query);
    }
  }
}

/// <summary>
/// Information about a component.
/// </summary>
internal sealed class ComponentInfo
{
  public static ComponentInfo Create(Type type) => new()
  {
    Type = type,
    IsReadOnly = !type.IsByRef,
    IsReadWrite = type.IsByRef
  };

  public required Type Type { get; init; }
  public required bool IsReadOnly { get; init; }
  public required bool IsReadWrite { get; init; }
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
