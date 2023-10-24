using Surreal.Colors;
using Surreal.Graphics.Gizmos;
using Surreal.Graphics.Rendering;
using Surreal.Maths;
using Surreal.Timing;

namespace Surreal.Scenes.Spatial.Physics;

/// <summary>
/// A rigidbody in 2D space.
/// </summary>
public class RigidBody2D : PhysicsBody2D, IGizmoObject
{
  private Vector2 _velocity;
  private float _torque;
  private bool _isDirty = true;

  /// <summary>
  /// The velocity of the body.
  /// </summary>
  public Vector2 Velocity
  {
    get => _velocity;
    set
    {
      if (SetField(ref _velocity, value))
      {
        _isDirty = true;
      }
    }
  }

  /// <summary>
  /// The torque applied to the body.
  /// </summary>
  public float Torque
  {
    get => _torque;
    set
    {
      if (SetField(ref _torque, value))
      {
        _isDirty = true;
      }
    }
  }

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    if (_isDirty)
    {
      PhysicsWorld.SetBodyVelocity(PhysicsBody, _velocity);
      PhysicsWorld.SetBodyTorque(PhysicsBody, _torque);

      _isDirty = false;
    }

    _velocity = PhysicsWorld.GetBodyVelocity(PhysicsBody);
    _torque = PhysicsWorld.GetBodyTorque(PhysicsBody);

    PhysicsWorld.GetBodyTransform(
      handle: PhysicsBody,
      position: out var position,
      rotation: out var rotation,
      scale: out var scale
    );

    GlobalPosition = position;
    GlobalRotation = Angle.FromRadians(rotation);
    GlobalScale = scale;
  }

  void IGizmoObject.RenderGizmos(in RenderFrame frame, GizmoBatch gizmos)
  {
    var color = Color.Lerp(Color.Yellow, Color.Green, Velocity.Length() / 50f);

    gizmos.DrawLine(GlobalPosition, GlobalPosition + Velocity, color);
  }
}
