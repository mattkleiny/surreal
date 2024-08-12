using Surreal.Timing;

namespace Surreal.Scenes.Canvas.Physics;

/// <summary>
/// A rigidbody in 2D space.
/// </summary>
public class RigidBody2D : PhysicsBody2D
{
  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    GlobalPosition = PhysicsWorld.GetBodyPosition(PhysicsBody);
  }
}
