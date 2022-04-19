using Isaac.Core.Actors.Components;
using Surreal.Aspects;
using Surreal.Components;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Permits physics in the game world.</summary>
public sealed class PhysicsSystem : IteratingSystem
{
  public PhysicsSystem()
    : base(Aspect.Of<Transform, RigidBody>())
  {
  }

  public Vector2 Gravity { get; set; } = new(0f, -9.8f);
}
