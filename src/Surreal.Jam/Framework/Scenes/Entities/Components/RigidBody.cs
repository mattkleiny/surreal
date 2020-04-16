using System.Numerics;

namespace Surreal.Framework.Scenes.Entities.Components
{
  public struct RigidBody : IComponent
  {
    public Vector2 Velocity;
    public Vector2 Acceleration;
    public float   Mass;
  }
}
