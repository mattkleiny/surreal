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
  ref TComponent GetComponent(EntityId entity);
  void AddComponent(EntityId entity, TComponent component);
  void RemoveComponent(EntityId entity);
}

/// <summary>
/// Sparse, dictionary-based storage for components.
/// </summary>
public sealed class SparseComponentStorage<TComponent> : IComponentStorage<TComponent>
  where TComponent : IComponent<TComponent>
{
  private readonly Dictionary<EntityId, TComponent> _components = new();

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
  public static EntityQuery Empty { get; } = new();

  private HashSet<ComponentType> _includeMask = [];
  private HashSet<ComponentType> _excludeMask = [];

  public EntityQuery Include<TComponent>()
    where TComponent : IComponent => Include(TComponent.ComponentType);

  public EntityQuery Include(ComponentType type)
  {
    _includeMask.Add(type);
    return this;
  }

  public EntityQuery Exclude<TComponent>()
    where TComponent : IComponent => Exclude(TComponent.ComponentType);

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
  private readonly Arena<Entity> _entities = new();
  private readonly Dictionary<ComponentType, IComponentStorage> _components = new();
  private readonly MultiDictionary<Type, IEntitySystem> _systems = new();

  /// <summary>
  /// The services available to the world.
  /// </summary>
  public IServiceProvider Services => services;

  public EntityId SpawnEntity()
  {
    var index = _entities.Add(new Entity
    {
      IsAlive = true,
    });

    return EntityId.FromArenaIndex(index);
  }

  public void DespawnEntity(EntityId entityId)
  {
    if (_entities.TryGetValue(entityId, out var entity))
    {
      entity.IsAlive = false;
    }
  }

  public void AddSystem<TEvent>(IEntitySystem system)
  {
    _systems.Add(typeof(TEvent), system);
  }

  public void RemoveSystem<TEvent>(IEntitySystem system)
  {
    _systems.Remove(typeof(TEvent), system);
  }

  public void Execute<TEvent>(in TEvent @event)
  {
    foreach (var system in _systems[typeof(TEvent)])
    {
      var typedSystem = (IEntitySystem<TEvent>)system;

      typedSystem.Execute(@event, this);
    }
  }

  public ref TComponent GetComponent<TComponent>(EntityId entity)
    where TComponent : IComponent<TComponent>
  {
    return ref GetStorage<TComponent>().GetComponent(entity);
  }

  public void AddComponent<TComponent>(EntityId entity, TComponent component)
    where TComponent : IComponent<TComponent>
  {
    GetStorage<TComponent>().AddComponent(entity, component);
  }

  public void RemoveComponent<TComponent>(EntityId entity)
    where TComponent : IComponent<TComponent>
  {
    GetStorage<TComponent>().RemoveComponent(entity);
  }

  public ReadOnlySlice<EntityId> Query(EntityQuery query)
  {
    var entityIds = new List<EntityId>();

    foreach (var (index, _) in _entities.EnumerateWithId())
    {
      entityIds.Add(EntityId.FromArenaIndex(index));
    }

    return entityIds;
  }

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
/// An <see cref="IEntitySystem"/> that processes entities based on a delegate.
/// </summary>
internal sealed class DelegateEntitySystem<TEvent>(Delegate @delegate) : IEntitySystem<TEvent>
{
  private delegate void SystemDelegate(in TEvent @event, EntityWorld world);

  // TODO: handle the case where there is no per-entity data

  private readonly EntityQuery _query = CreateQuery(@delegate);
  private readonly SystemDelegate _method = CreateMethod(@delegate);

  public void Execute(in TEvent @event, EntityWorld world)
  {
    _method.Invoke(in @event, world);
  }

  private static SystemDelegate CreateMethod(Delegate @delegate)
  {
    // TODO: find a way to do this without emitting IL?
    var method = new DynamicMethod(
      name: "ExecuteSystem",
      returnType: typeof(void),
      parameterTypes: [typeof(TEvent), typeof(EntityWorld)],
      owner: typeof(DelegateEntitySystem<TEvent>)
    );

    var builder = method.GetILGenerator();
    var parameterInfos = @delegate.Method.GetParameters();

    foreach (var parameterInfo in parameterInfos)
    {
      // unpack ref parameters
      var parameterType = parameterInfo.ParameterType;
      if (parameterType.IsByRef)
      {
        parameterType = parameterType.GetElementType()!;
      }

      // handle component inputs
      if (parameterType.IsAssignableTo(typeof(IComponent)))
      {
        // inject the component
      }
      else if (parameterType == typeof(TEvent))
      {
        // inject the event
      }
      else if (parameterType == typeof(EntityWorld))
      {
        // inject the world
      }
      else
      {
        // inject the service
      }
    }

    return (SystemDelegate)method.CreateDelegate(typeof(SystemDelegate));
  }

  private static EntityQuery CreateQuery(Delegate @delegate)
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

public static class DelegateEntitySystemExtensions
{
  /// <summary>
  /// Adds a system to the world that processes entities based on the given delegate.
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
}
