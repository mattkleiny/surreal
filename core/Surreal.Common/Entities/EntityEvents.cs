
using Surreal.Timing;

namespace Surreal.Entities;

/// <summary>
/// An event that implicitly occurs before another event.
/// </summary>
public record struct Before<T>(T Event);

/// <summary>
/// An event that implicitly occurs after another event.
/// </summary>
public record struct After<T>(T Event);

/// <summary>
/// Represents a variable tick event.
/// </summary>
public record struct VariableTick(DeltaTime DeltaTime);

/// <summary>
/// Represents a fixed tick event.
/// </summary>
public record struct FixedTick(DeltaTime DeltaTime);

/// <summary>
/// An event that indicates an entity has been spawned.
/// </summary>
public record struct Spawned(EntityId Entity);

/// <summary>
/// An event that indicates an entity has been despawned.
/// </summary>
public record struct Despawned(EntityId Entity);

/// <summary>
/// An event that indicates a component has been added to an entity.
/// </summary>
public record struct Added<TComponent>(EntityId Entity, TComponent Component);

/// <summary>
/// An event that indicates a component has been removed from an entity.
/// </summary>
public record struct Removed<TComponent>(EntityId Entity, TComponent Component);
