using Surreal.Colors;
using Surreal.Graphics.Gizmos;
using Surreal.Graphics.Rendering;
using Surreal.Timing;

namespace Surreal.Scenes.Spatial.Physics;

/// <summary>
/// A rigidbody in 2D space.
/// </summary>
public class RigidBody2D : PhysicsBody2D, IGizmoObject
{
  /// <summary>
  /// The velocity of the body.
  /// </summary>
  public Vector2 Velocity => PhysicsWorld.GetBodyVelocity(PhysicsBody);

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    GlobalPosition = PhysicsWorld.GetBodyPosition(PhysicsBody);
  }

  void IGizmoObject.RenderGizmos(in RenderFrame frame, GizmoBatch gizmos)
  {
    var color = Color.Lerp(Color.Yellow, Color.Green, Velocity.Length() / 50f);

    gizmos.DrawLine(GlobalPosition, GlobalPosition + Velocity, color);
  }
}
