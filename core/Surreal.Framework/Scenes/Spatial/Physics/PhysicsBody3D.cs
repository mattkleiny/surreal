using Surreal.Physics;

namespace Surreal.Scenes.Spatial.Physics;

/// <summary>
/// A physical body in 3D space.
/// </summary>
public abstract class PhysicsBody3D : Node3D
{
  private IPhysicsWorld3d? _physicsWorld;

  /// <summary>
  /// The <see cref="PhysicsHandle"/> for the body.
  /// </summary>
  public PhysicsHandle Handle { get; private set; }

  /// <summary>
  /// The <see cref="IPhysicsWorld3d"/> that the body belongs to.
  /// </summary>
  public IPhysicsWorld3d PhysicsWorld
  {
    get
    {
      if (_physicsWorld == null)
      {
        var world = Root.Physics as IPhysicsWorld3d;

        _physicsWorld = world ?? throw new InvalidOperationException("A 3d physics world is not available");
      }

      return _physicsWorld;
    }
  }

  protected override void OnEnterTree()
  {
    base.OnEnterTree();

    Handle = PhysicsWorld.CreateBody(GlobalPosition);
  }

  protected override void OnExitTree()
  {
    PhysicsWorld.DeleteBody(Handle);

    base.OnExitTree();

    _physicsWorld = null;
  }
}
