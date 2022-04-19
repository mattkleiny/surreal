using Isaac.Core.Actors.Components;
using Surreal.Aspects;
using Surreal.Components;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Permits physics in the game world.</summary>
public sealed class PhysicsSystem : IteratingSystem
{
  private static Aspect Aspect { get; } = new Aspect()
    .With<Transform>()
    .With<RigidBody>();

  public PhysicsSystem()
    : base(Aspect)
  {
  }

  public Vector2 Gravity { get; set; } = new(0f, -9.8f);
}
