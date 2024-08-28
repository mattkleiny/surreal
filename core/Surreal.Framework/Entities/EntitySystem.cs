using System.Linq.Expressions;
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
  public static ComponentType FromType(Type type)
  {
    return new ComponentType(type.Name, ComponentTypeMask.FromType(type));
  }

  private ComponentType(string name, ComponentTypeMask mask)
  {
    Name = name;
    Mask = mask;
  }

  /// <summary>
  /// The name of the component type.
  /// </summary>
  public string Name { get; }

  /// <summary>
  /// The mask that represents this component type.
  /// </summary>
  internal ComponentTypeMask Mask { get; }

  public override string ToString() => Name;
}

/// <summary>
/// A mask that represents a set of component types.
/// </summary>
internal readonly record struct ComponentTypeMask(ulong Mask)
{
  public static ComponentTypeMask Empty => default;

  public static ComponentTypeMask FromType(Type type) => new(1UL << type.GetHashCode());

  public static ComponentTypeMask operator |(ComponentTypeMask left, ComponentTypeMask right) => new(left.Mask | right.Mask);
  public static ComponentTypeMask operator &(ComponentTypeMask left, ComponentTypeMask right) => new(left.Mask & right.Mask);
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
  void Execute(in TEvent @event, IEntityQueryProvider provider);
}

/// <summary>
/// A query that selects entities based on their components.
/// </summary>
public sealed class EntityQuery
{
  public static EntityQuery Empty { get; } = new();

  private ComponentTypeMask _includeMask;
  private ComponentTypeMask _excludeMask;

  public EntityQuery Include<TComponent>()
    where TComponent : IComponent => Include(TComponent.ComponentType);

  public EntityQuery Include(ComponentType type)
  {
    _includeMask |= type.Mask;
    return this;
  }

  public EntityQuery Exclude<TComponent>()
    where TComponent : IComponent => Exclude(TComponent.ComponentType);

  public EntityQuery Exclude(ComponentType type)
  {
    _excludeMask |= type.Mask;
    return this;
  }
}

/// <summary>
/// A provider that can query entities.
/// </summary>
public interface IEntityQueryProvider
{
  /// <summary>
  /// Matches entities based on the given query.
  /// </summary>
  ReadOnlySlice<EntityId> Query(EntityQuery query);

  /// <summary>
  /// Gets the component of the given type for the given entity.
  /// </summary>
  ref TComponent GetComponent<TComponent>(EntityId entity)
    where TComponent : IComponent<TComponent>;
}

/// <summary>
/// A world that contains entities and their components.
/// </summary>
public sealed class EntityWorld : IEntityQueryProvider
{
  private readonly Arena<Entity> _entities = new();
  private readonly Dictionary<ComponentType, IComponentStorage> _components = new();
  private readonly MultiDictionary<Type, IEntitySystem> _systems = new();

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
public sealed class DelegateEntitySystem<TEvent>(Delegate @delegate) : IEntitySystem<TEvent>
{
  private readonly EntityQuery _query = CreateQuery(@delegate);

  public void Execute(in TEvent @event, IEntityQueryProvider provider)
  {
    // TODO: use an expression tree to compile the delegate instead
    var parameterInfos = @delegate.Method.GetParameters();
    var parameterFactory = new Func<EntityId, object?>[parameterInfos.Length];

    foreach (var parameterInfo in parameterInfos)
    {
      // unwrap ref parameters
      var parameterType = parameterInfo.ParameterType;
      if (parameterInfo.ParameterType.IsByRef)
      {
        parameterType = parameterType.GetElementType()!;
      }

      if (parameterType == typeof(TEvent))
      {
        var copy = @event;
        parameterFactory[parameterInfo.Position] = _ => copy;
      }

      if (parameterType.IsAssignableTo(typeof(IComponent)))
      {
        var callback =
          typeof(IEntityQueryProvider)
            .GetMethod(nameof(IEntityQueryProvider.GetComponent))!
            .MakeGenericMethod(parameterType);

        parameterFactory[parameterInfo.Position] = id => callback.Invoke(provider, [id]);
      }
    }

    foreach (var entityId in provider.Query(_query))
    {
      var parameters = parameterFactory.Select(factory => factory(entityId)).ToArray();

      @delegate.DynamicInvoke(parameters);
    }
  }

  /// <summary>
  /// Creates a <see cref="EntityQuery"/> for the parameters of the given delegate.
  /// </summary>
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
