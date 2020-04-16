using System.Diagnostics;
using System.Numerics;

namespace Surreal.Framework.Scenes.Entities.Components
{
  [DebuggerDisplay("Transform {Position} @ {Rotation}°")]
  public struct Transform : IComponent
  {
    public Vector2 Position;
    public Vector2 Scale;
    public float   Rotation;
  }
}