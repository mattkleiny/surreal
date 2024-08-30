using Surreal.Collections;
using Surreal.Collections.Slices;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal.Entities;

// a bunch of built-in events that can be used to trigger systems
public record struct Before<T>(T Event); // implicit before event wrapper
public record struct After<T>(T Event); // implicit after event wrapper 
public record struct TickEvent(DeltaTime DeltaTime);
public record struct FixedTickEvent(DeltaTime DeltaTime);

/// <summary>
/// Identifies an entity in the world.
/// </summary>
public record struct EntityId(ulong Id)
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static EntityId FromArenaIndex(ArenaIndex index)
  {
    return new EntityId(index);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator ArenaIndex(EntityId id)
  {
    return ArenaIndex.FromUlong(id.Id);
  }
}

/// <summary>
/// Represents a type of component.
/// </summary>
public sealed record ComponentType
{
  /// <summary>
  /// Creates a new <see cref="ComponentType"/> from the given CLR type.
  /// </summary>
  public static ComponentType FromType(Type type) => new() { Name = type.Name };

  /// <summary>
  /// The name of the component type.
  /// </summary>
  public required string Name { get; init; }

  public override string ToString() => $"ComponentType({Name})";
}

/// <summary>
/// Represents a component that can be attached to an entity.
/// </summary>
public interface IComponent
{
  /// <summary>
  /// The type of the component.
  /// </summary>
  static abstract ComponentType ComponentType { get; }
}

public interface IComponent<TSelf> : IComponent
  where TSelf : IComponent<TSelf>
{
  /// <summary>
  /// Creates a new instance of the storage for this component type.
  /// </summary>
  static virtual IComponentStorage<TSelf> CreateStorage()
  {
    return new SparseComponentStorage<TSelf>();
  }

  /// <inheritdoc/>
  static ComponentType IComponent.ComponentType => ComponentType.FromType(typeof(TSelf));
}

/// <summary>
/// Storage for components of a specific type.
/// </summary>
public interface IComponentStorage
{
  /// <summary>
  /// The type of component stored in this storage.
  /// </summary>
  ComponentType ComponentType { get; }
}

public interface IComponentStorage<TComponent> : IComponentStorage
  where TComponent : IComponent<TComponent>
{
  /// <inheritdoc/>
  ComponentType IComponentStorage.ComponentType => TComponent.ComponentType;

  /// <summary>
  /// Determines if the given entity has a component of this type.
  /// </summary>
  bool HasComponent(EntityId entity);

  /// <summary>
  /// Gets the component of this type for the given entity.
  /// </summary>
  ref TComponent GetComponent(EntityId entity);

  /// <summary>
  /// Adds a component of this type to the given entity.
  /// </summary>
  void AddComponent(EntityId entity, TComponent component);

  /// <summary>
  /// Removes the component of this type from the given entity.
  /// </summary>
  void RemoveComponent(EntityId entity);
}

/// <summary>
/// Sparse, dictionary-based storage for components.
/// </summary>
public sealed class SparseComponentStorage<TComponent> : IComponentStorage<TComponent>
  where TComponent : IComponent<TComponent>
{
  private readonly Dictionary<EntityId, TComponent> _components = new();

  public bool HasComponent(EntityId entity)
  {
    return _components.ContainsKey(entity);
  }

  public ref TComponent GetComponent(EntityId entity)
  {
    return ref CollectionsMarshal.GetValueRefOrNullRef(_components, entity);
  }

  public void AddComponent(EntityId entity, TComponent component)
  {
    _components[entity] = component;
  }

  public void RemoveComponent(EntityId entity)
  {
    _components.Remove(entity);
  }
}

/// <summary>
/// Represents a system that processes entities.
/// </summary>
public interface IEntitySystem
{
  /// <summary>
  /// The name of the system.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// The type of event that triggers this system.
  /// </summary>
  Type EventType { get; }
}

public interface IEntitySystem<TEvent> : IEntitySystem
{
  /// <inheritdoc/>
  Type IEntitySystem.EventType => typeof(TEvent);

  /// <summary>
  /// Executes the system with the given entity query provider in response to the given event.
  /// </summary>
  void Execute(in TEvent @event, EntityWorld world);
}

/// <summary>
/// A query that selects entities based on their components.
/// </summary>
public sealed class EntityQuery
{
  /// <summary>
  /// A no-op query that selects no entities.
  /// </summary>
  public static EntityQuery Empty { get; } = new();

  private HashSet<ComponentType> _included = [];
  private HashSet<ComponentType> _excluded = [];

  /// <summary>
  /// Determines if the query is empty.
  /// </summary>
  public bool IsEmpty => _included.Count == 0 && _excluded.Count == 0;

  /// <summary>
  /// Determines if the query includes entities with the given component type.
  /// </summary>
  public bool HasInclude(ComponentType type) => _included.Contains(type);

  /// <summary>
  /// Determines if the query excludes entities with the given component type.
  /// </summary>
  public bool HasExclude(ComponentType type) => _excluded.Contains(type);

  /// <summary>
  /// Determines if the query matches the given set of component types.
  /// </summary>
  public bool Matches(HashSet<ComponentType> componentTypes)
  {
    foreach (var include in _included)
    {
      if (!componentTypes.Contains(include))
      {
        return false;
      }
    }

    foreach (var exclude in _excluded)
    {
      if (componentTypes.Contains(exclude))
      {
        return false;
      }
    }

    return true;
  }

  /// <summary>
  /// Adds a predicate to the query that includes entities with the given component type.
  /// </summary>
  public EntityQuery Include<TComponent>()
    where TComponent : IComponent => Include(TComponent.ComponentType);

  /// <summary>
  /// Adds a predicate to the query that includes entities with the given component type.
  /// </summary>
  public EntityQuery Include(ComponentType type)
  {
    _included.Add(type);
    return this;
  }

  /// <summary>
  /// Adds a predicate to the query that excludes entities with the given component type.
  /// </summary>
  public EntityQuery Exclude<TComponent>()
    where TComponent : IComponent => Exclude(TComponent.ComponentType);

  /// <summary>
  /// Adds a predicate to the query that excludes entities with the given component type.
  /// </summary>
  public EntityQuery Exclude(ComponentType type)
  {
    _excluded.Add(type);
    return this;
  }
}

/// <summary>
/// A world that contains entities and their components.
/// </summary>
public sealed class EntityWorld : IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<EntityWorld>();
  private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<EntityWorld>();

  private readonly Arena<Entity> _entities = [];
  private readonly Dictionary<ComponentType, IComponentStorage> _components = [];
  private readonly MultiDictionary<Type, IEntitySystem> _systems = new();
  private readonly IServiceProvider services;

  /// <summary>
  /// Builds a new <see cref="EntityWorld"/> from the given services.
  /// <para/>
  /// If <paramref name="includeRegisteredSystems"/> is true, all registered <see cref="IEntitySystem"/>s will be included.
  /// </summary>
  public EntityWorld(IServiceProvider services, bool includeRegisteredSystems = true)
  {
    this.services = services;

    if (includeRegisteredSystems)
    {
      foreach (var system in services.GetServices<IEntitySystem>())
      {
        AddSystem(system);
      }
    }
  }


  /// <summary>
  /// The services available to the world.
  /// </summary>
  public IServiceProvider Services => services;

  /// <summary>
  /// Determines if the world contains the given entity and it is still alive.
  /// </summary>
  public bool HasEntity(EntityId entityId)
  {
    if (_entities.TryGetValue(entityId, out var entity))
    {
      return entity.IsAlive;
    }

    return false;
  }

  /// <summary>
  /// Spawns a new entity in the world.
  /// </summary>
  public EntityId SpawnEntity()
  {
    var index = _entities.Add(new Entity());
    var entity = EntityId.FromArenaIndex(index);

    Log.Trace($"Spawned entity {entity}");

    return entity;
  }

  /// <summary>
  /// Despawns the entity with the given ID.
  /// </summary>
  public void DespawnEntity(EntityId entityId)
  {
    if (_entities.TryGetValue(entityId, out var entity))
    {
      entity.IsAlive = false;

      Log.Trace($"Marking entity {entityId} for despawn");
    }
  }

  /// <summary>
  /// Processes all deferred changes to the world.
  /// </summary>
  public void FlushChanges()
  {
    foreach (var (index, entity) in _entities.EnumerateWithId())
    {
      if (!entity.IsAlive)
      {
        _entities.Remove(index);

        Log.Trace($"Despawned entity {EntityId.FromArenaIndex(index)}");
      }
    }
  }

  /// <summary>
  /// Adds a system to the world.
  /// </summary>
  public void AddSystem(IEntitySystem system)
  {
    _systems.Add(system.EventType, system);

    Log.Trace($"Added system {system.Name}");
  }

  /// <summary>
  /// Removes the given system from the world.
  /// </summary>
  public void RemoveSystem(IEntitySystem system)
  {
    _systems.Remove(system.EventType, system);

    Log.Trace($"Removed system {system.Name}");
  }

  /// <summary>
  /// Executes the given event on all systems that process it.
  /// </summary>
  public void Execute<TEvent>(in TEvent @event)
  {
    // execute the event on all systems that process it
    ExecuteEvent(new Before<TEvent>(@event));
    ExecuteEvent(@event);
    ExecuteEvent(new After<TEvent>(@event));
  }

  /// <summary>
  /// Executes the given event on all systems that process it.
  /// </summary>
  private void ExecuteEvent<TEvent>(in TEvent @event)
  {
    using var _ = Profiler.Track(nameof(TEvent));

    foreach (var system in _systems[typeof(TEvent)])
    {
      if (system is not IEntitySystem<TEvent> typedSystem)
      {
        throw new InvalidOperationException($"System {system.Name} does not process events of type {typeof(TEvent)}.");
      }
      
      using var __ = Profiler.Track(system.Name);

      typedSystem.Execute(@event, this);
    }
  }

  /// <summary>
  /// Queries for all entities that match the given query.
  /// </summary>
  public ReadOnlySlice<EntityId> Query(EntityQuery query)
  {
    var entityIds = new List<EntityId>();

    foreach (var (index, entity) in _entities.EnumerateWithId())
    {
      if (entity.IsMatch(query))
      {
        entityIds.Add(EntityId.FromArenaIndex(index));
      }
    }

    return entityIds;
  }

  /// <summary>
  /// Determines if the given entity has a component of the given type.
  /// </summary>
  public bool HasComponent<TComponent>(EntityId entityId)
    where TComponent : IComponent<TComponent>
  {
    if (!_entities.Contains(entityId))
    {
      throw new InvalidOperationException($"Entity {entityId} does not exist.");
    }

    return GetStorage<TComponent>().HasComponent(entityId);
  }

  /// <summary>
  /// Gets the component of the given type for the given entity.
  /// </summary>
  public ref TComponent GetComponent<TComponent>(EntityId entityId)
    where TComponent : IComponent<TComponent>
  {
    if (!_entities.Contains(entityId))
    {
      throw new InvalidOperationException($"Entity {entityId} does not exist.");
    }

    return ref GetStorage<TComponent>().GetComponent(entityId);
  }

  /// <summary>
  /// Adds a component of the given type to the given entity.
  /// </summary>
  public void AddComponent<TComponent>(EntityId entityId, TComponent component)
    where TComponent : IComponent<TComponent>
  {
    if (!_entities.TryGetValue(entityId, out var entity))
    {
      throw new InvalidOperationException($"Entity {entityId} does not exist.");
    }

    if (entity.ComponentTypes.Add(TComponent.ComponentType))
    {
      GetStorage<TComponent>().AddComponent(entityId, component);

      Log.Trace($"Added component {TComponent.ComponentType} to entity {entityId}");
    }
  }

  /// <summary>
  /// Removes the component of the given type from the given entity.
  /// </summary>
  public void RemoveComponent<TComponent>(EntityId entityId)
    where TComponent : IComponent<TComponent>
  {
    if (!_entities.TryGetValue(entityId, out var entity))
    {
      throw new InvalidOperationException($"Entity {entityId} does not exist.");
    }

    if (entity.ComponentTypes.Remove(TComponent.ComponentType))
    {
      GetStorage<TComponent>().RemoveComponent(entityId);

      Log.Trace($"Removed component {TComponent.ComponentType} from entity {entityId}");
    }
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    foreach (var system in _systems.Values)
    {
      if (system is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    foreach (var storage in _components.Values)
    {
      if (storage is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    _entities.Clear();
    _components.Clear();
    _systems.Clear();
  }

  /// <summary>
  /// Gets the storage for the given component type.
  /// </summary>
  private IComponentStorage<TComponent> GetStorage<TComponent>()
    where TComponent : IComponent<TComponent>
  {
    if (!_components.TryGetValue(TComponent.ComponentType, out var storage))
    {
      storage = TComponent.CreateStorage();

      _components[TComponent.ComponentType] = storage;
    }

    return (IComponentStorage<TComponent>)storage;
  }

  /// <summary>
  /// An entity in the world.
  /// </summary>
  private sealed record Entity
  {
    /// <summary>
    /// True if this entity is still alive; false if it has been despawned and is pending removal.
    /// </summary>
    public bool IsAlive { get; set; } = true;

    /// <summary>
    /// All of the components attached to this entity.
    /// </summary>
    public HashSet<ComponentType> ComponentTypes { get; } = new();

    /// <summary>
    /// Determines if the entity matches the given query.
    /// </summary>
    public bool IsMatch(EntityQuery query)
    {
      return query.Matches(ComponentTypes);
    }
  }
}

/// <summary>
/// Helpers for working with <see cref="EntityWorld"/>s.
/// </summary>
public static class EntityWorldExtensions
{
  /// <summary>
  /// Registers a system that processes entities based on the given <see cref="Delegate"/>.
  /// </summary>
  public static void AddSystem<TEvent>(this IServiceRegistry registry, Delegate @delegate)
  {
    registry.AddService<IEntitySystem>(new DelegateEntitySystem<TEvent>(@delegate));
  }

  /// <summary>
  /// Adds a system to the world that processes entities based on the given <see cref="Delegate"/>.
  /// </summary>
  public static IDisposable AddSystem<TEvent>(this EntityWorld world, Delegate @delegate)
  {
    var system = new DelegateEntitySystem<TEvent>(@delegate);

    world.AddSystem(system);

    return Disposables.Anonymous(() =>
    {
      world.RemoveSystem(system);
    });
  }

  /// <summary>
  /// An <see cref="IEntitySystem"/> that processes entities based on a delegate.
  /// </summary>
  private sealed class DelegateEntitySystem<TEvent>(Delegate @delegate) : IEntitySystem<TEvent>
  {
    /// <summary>
    /// A delegate that processes the system execution.
    /// </summary>
    private delegate void SystemDelegate(in TEvent @event, EntityWorld world);

    private readonly SystemDelegate _method = BuildSystemDelegate(@delegate);

    /// <inheritdoc/>
    public string Name => @delegate.Method.Name;

    /// <inheritdoc/>
    public void Execute(in TEvent @event, EntityWorld world)
    {
      _method.Invoke(in @event, world);
    }

    /// <summary>
    /// Builds a system delegate that will execute the given delegate method with the appropriate parameters.
    /// </summary>
    private static SystemDelegate BuildSystemDelegate(Delegate @delegate)
    {
      if (@delegate.Method.ReturnType != typeof(void))
      {
        throw new ArgumentException("Delegate must return void.");
      }

      // find out if any of the parameters accesses components
      //
      // if they do, we need to embed the query logic into the method
      var isUsingQuery = false;
      var parameterInfos = @delegate.Method.GetParameters();

      for (var i = 0; i < parameterInfos.Length; i++)
      {
        var parameterInfo = parameterInfos[i];
        var parameterType = parameterInfo.ParameterType;

        if (parameterType.IsByRef)
          parameterType = parameterType.GetElementType()!;

        if (parameterType.IsAssignableTo(typeof(IComponent)))
        {
          isUsingQuery = true;
          break;
        }
      }

      if (!isUsingQuery)
      {
        return BuildDirectMethod(@delegate, parameterInfos);
      }

      return BuildQueryMethod(@delegate, parameterInfos);
    }

    /// <summary>
    /// Builds a system delegate that will execute the given 'direct' delegate (i.e. no loop unwrapping).
    /// <para/>
    /// This method uses a reflection-based invocaion since the we're not building a hot loop over entities.
    /// </summary>
    private static SystemDelegate BuildDirectMethod(Delegate @delegate, ParameterInfo[] parameterInfos)
    {
      // TODO: build an optimized method that doesn't use reflection
      var parameters = new object?[parameterInfos.Length];

      return (in TEvent @event, EntityWorld world) =>
      {
        for (var i = 0; i < parameterInfos.Length; i++)
        {
          var parameterInfo = parameterInfos[i];
          var parameterType = parameterInfo.ParameterType;

          if (parameterType.IsByRef)
            parameterType = parameterType.GetElementType()!;

          if (parameterType == typeof(TEvent))
            parameters[i] = @event;
          else if (parameterType == typeof(EntityWorld))
            parameters[i] = world;
          else
            parameters[i] = world.Services.GetService(parameterType);
        }

        @delegate.DynamicInvoke(parameters);
      };
    }

    /// <summary>
    /// Builds a system delegate that will execute the given delegate method with the appropriate parameters.
    /// <para/>
    /// This method builds a dynamic method that will execute the delegate method inside of a loop for each
    /// entity that matches the query inferred by it's method parameters.
    /// </summary>
    private static SystemDelegate BuildQueryMethod(Delegate @delegate, ParameterInfo[] parameterInfos)
    {
      // TODO: build an optimized method that doesn't use reflection
      var parameters = new object?[parameterInfos.Length];

      return (in TEvent @event, EntityWorld world) =>
      {
        // populate root parameters
        for (var i = 0; i < parameterInfos.Length; i++)
        {
          var parameterInfo = parameterInfos[i];
          var parameterType = parameterInfo.ParameterType;

          if (parameterType.IsByRef)
            parameterType = parameterType.GetElementType()!;

          if (parameterType == typeof(TEvent))
            parameters[i] = @event;
          else if (parameterType == typeof(EntityWorld))
            parameters[i] = world;
          else if (parameterType == typeof(EntityId))
            continue; // ignore per-entity data, we'll populate that later
          else if (parameterType.IsAssignableTo(typeof(IComponent)))
            // build the generic method once, we'll populate the entity-specific data later
            parameters[i] = typeof(EntityWorld).GetMethod(nameof(EntityWorld.GetComponent))!.MakeGenericMethod(parameterType);
          else
            parameters[i] = world.Services.GetService(parameterType);
        }

        foreach (var entity in world.Query(BuildQuery(@delegate)))
        {
          // populate per-entity parameters
          for (var i = 0; i < parameterInfos.Length; i++)
          {
            var parameterInfo = parameterInfos[i];
            var parameterType = parameterInfo.ParameterType;

            if (parameterType.IsByRef)
              parameterType = parameterType.GetElementType()!;

            if (parameterType == typeof(EntityId))
              parameters[i] = entity;
            else if (parameterType.IsAssignableTo(typeof(IComponent)) && parameters[i] is MethodInfo getter)
              parameters[i] = getter.Invoke(world, [entity]);
          }

          @delegate.DynamicInvoke(parameters);
        }
      };
    }

    /// <summary>
    /// Builds an <see cref="EntityQuery"/> that matches the components accessed by the given delegate.
    /// </summary>
    private static EntityQuery BuildQuery(Delegate @delegate)
    {
      var query = new EntityQuery();

      foreach (var parameterInfo in @delegate.Method.GetParameters())
      {
        var parameterType = parameterInfo.ParameterType;

        if (parameterInfo.ParameterType.IsByRef)
        {
          parameterType = parameterType.GetElementType()!;
        }

        if (parameterType.IsAssignableTo(typeof(IComponent)))
        {
          query = query.Include(ComponentType.FromType(parameterType));
        }
      }

      return query;
    }
  }
}
