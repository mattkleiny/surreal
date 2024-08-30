using System.Reflection.Emit;
using Surreal.Collections;
using Surreal.Collections.Slices;

namespace Surreal.Entities;

/// <summary>
/// Represents an entity in the world.
/// </summary>
public record struct EntityId(ulong Id)
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static EntityId FromArenaIndex(ArenaIndex index) => new(index);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator ArenaIndex(EntityId id) => ArenaIndex.FromUlong(id.Id);
}

/// <summary>
/// Represents a type of component.
/// </summary>
public sealed record ComponentType
{
  /// <summary>
  /// Creates a new <see cref="ComponentType"/> from the given CLR type.
  /// </summary>
  public static ComponentType FromType(Type type) => new()
  {
    Name = type.Name
  };

  public required string Name { get; init; }

  public override string ToString()
  {
    return $"ComponentType({Name})";
  }
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

/// <summary>
/// A strongly-typed <see cref="IComponent"/> that can be attached to an entity.
/// </summary>
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

  static ComponentType IComponent.ComponentType => ComponentType.FromType(typeof(TSelf));
}

/// <summary>
/// Storage for components of a specific type.
/// </summary>
public interface IComponentStorage;

public interface IComponentStorage<TComponent> : IComponentStorage
  where TComponent : IComponent<TComponent>
{
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
public interface IEntitySystem;

public interface IEntitySystem<TEvent> : IEntitySystem
{
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

  private HashSet<ComponentType> _includeMask = [];
  private HashSet<ComponentType> _excludeMask = [];

  /// <summary>
  /// Determines if the query is empty.
  /// </summary>
  public bool IsEmpty => _includeMask.Count == 0 && _excludeMask.Count == 0;

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
    _includeMask.Add(type);
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
    _excludeMask.Add(type);
    return this;
  }
}

/// <summary>
/// A world that contains entities and their components.
/// </summary>
public sealed class EntityWorld(IServiceProvider services)
{
  private readonly Arena<Entity> _entities = [];
  private readonly Dictionary<ComponentType, IComponentStorage> _components = [];
  private readonly MultiDictionary<Type, IEntitySystem> _systems = new();

  /// <summary>
  /// The services available to the world.
  /// </summary>
  public IServiceProvider Services => services;

  /// <summary>
  /// Determines if the world contains the given entity.
  /// </summary>
  public bool HasEntity(EntityId entityId)
  {
    return _entities.Contains(entityId);
  }

  /// <summary>
  /// Spawns a new entity in the world.
  /// </summary>
  public EntityId SpawnEntity()
  {
    var index = _entities.Add(new Entity
    {
      IsAlive = true,
    });

    return EntityId.FromArenaIndex(index);
  }

  /// <summary>
  /// Despawns the entity with the given ID.
  /// </summary>
  public void DespawnEntity(EntityId entityId)
  {
    if (_entities.TryGetValue(entityId, out var entity))
    {
      entity.IsAlive = false;
    }
  }

  /// <summary>
  /// Adds a system to the world that processes the given <typeparamref name="TEvent"/>.
  /// </summary>
  public void AddSystem<TEvent>(IEntitySystem system)
  {
    _systems.Add(typeof(TEvent), system);
  }

  /// <summary>
  /// Removes the given system from the world.
  /// </summary>
  public void RemoveSystem<TEvent>(IEntitySystem system)
  {
    _systems.Remove(typeof(TEvent), system);
  }

  /// <summary>
  /// Executes the given event on all systems that process it.
  /// </summary>
  public void Execute<TEvent>(in TEvent @event)
  {
    foreach (var system in _systems[typeof(TEvent)])
    {
      var typedSystem = (IEntitySystem<TEvent>)system;

      typedSystem.Execute(@event, this);
    }
  }

  /// <summary>
  /// Determines if the given entity has a component of the given type.
  /// </summary>
  public bool HasComponent<TComponent>(EntityId entity)
    where TComponent : IComponent<TComponent>
  {
    return GetStorage<TComponent>().HasComponent(entity);
  }

  /// <summary>
  /// Gets the component of the given type for the given entity.
  /// </summary>
  public ref TComponent GetComponent<TComponent>(EntityId entity)
    where TComponent : IComponent<TComponent>
  {
    return ref GetStorage<TComponent>().GetComponent(entity);
  }

  /// <summary>
  /// Adds a component of the given type to the given entity.
  /// </summary>
  public void AddComponent<TComponent>(EntityId entity, TComponent component)
    where TComponent : IComponent<TComponent>
  {
    GetStorage<TComponent>().AddComponent(entity, component);
  }

  /// <summary>
  /// Removes the component of the given type from the given entity.
  /// </summary>
  public void RemoveComponent<TComponent>(EntityId entity)
    where TComponent : IComponent<TComponent>
  {
    GetStorage<TComponent>().RemoveComponent(entity);
  }

  /// <summary>
  /// Queries for all entities that match the given query.
  /// </summary>
  public ReadOnlySlice<EntityId> Query(EntityQuery query)
  {
    var entityIds = new List<EntityId>();

    foreach (var (index, _) in _entities.EnumerateWithId())
    {
      entityIds.Add(EntityId.FromArenaIndex(index));
    }

    return entityIds;
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
  private sealed class Entity
  {
    public bool IsAlive { get; set; }
  }
}

/// <summary>
/// Helpers for working with <see cref="EntityWorld"/>s.
/// </summary>
public static class EntityWorldExtensions
{
  /// <summary>
  /// Adds a system to the world that processes entities based on the given <see cref="Delegate"/>.
  /// </summary>
  public static IDisposable AddSystem<TEvent>(this EntityWorld world, Delegate @delegate)
  {
    var system = new DelegateEntitySystem<TEvent>(@delegate);

    world.AddSystem<TEvent>(system);

    return Disposables.Anonymous(() =>
    {
      world.RemoveSystem<TEvent>(system);
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
      var method = new DynamicMethod(
        name: "Query",
        returnType: typeof(void),
        parameterTypes: [typeof(TEvent), typeof(EntityWorld)],
        owner: typeof(DelegateEntitySystem<TEvent>)
      );

      var query = BuildQuery(@delegate);
      var il = method.GetILGenerator();

      foreach (var parameterInfo in parameterInfos)
      {
        var parameterType = parameterInfo.ParameterType;
        if (parameterType.IsByRef)
          parameterType = parameterType.GetElementType()!;

        throw new NotImplementedException();
      }

      il.Emit(OpCodes.Call, @delegate.Method);
      il.Emit(OpCodes.Ret);

      return (SystemDelegate)method.CreateDelegate(typeof(SystemDelegate));
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
