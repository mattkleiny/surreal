using Surreal.Entities;

namespace Surreal.Physics;

/// <summary>
/// A component that adds rigidbody physics to an entity.
/// </summary>
public record struct Rigidbody(PhysicsHandle Handle) : IComponent<Rigidbody>;
