using Surreal.Physics;

namespace Surreal.Scenes.Spatial.Physics;

/// <summary>
/// A physical body in 2D space.
/// </summary>
public abstract class PhysicsBody2D : Node2D
{
  private IPhysicsWorld2d? _physicsWorld;

  /// <summary>
  /// The <see cref="PhysicsHandle"/> for the body.
  /// </summary>
  public PhysicsHandle PhysicsBody { get; private set; }

  /// <summary>
  /// The <see cref="IPhysicsWorld2d"/> that the body belongs to.
  /// </summary>
  public IPhysicsWorld2d PhysicsWorld
  {
    get
    {
      if (_physicsWorld == null)
      {
        var world = Owner?.PhysicsWorld as IPhysicsWorld2d;

        _physicsWorld = world ?? throw new InvalidOperationException("A 2d physics world is not available");
      }

      return _physicsWorld;
    }
  }

  protected override void OnEnterTree()
  {
    base.OnEnterTree();

    PhysicsBody = PhysicsWorld.CreateBody(GlobalPosition);
  }

  protected override void OnExitTree()
  {
    PhysicsWorld.DeleteBody(PhysicsBody);

    base.OnExitTree();

    _physicsWorld = null;
  }
}
