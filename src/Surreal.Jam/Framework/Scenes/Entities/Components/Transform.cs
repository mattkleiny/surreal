using System.Diagnostics;
using System.Numerics;
using Surreal.Mathematics;

namespace Surreal.Framework.Scenes.Entities.Components {
  [DebuggerDisplay("Transform {Position} @ {Rotation}°")]
  public struct Transform : IComponent {
    public Vector2 Position;
    public Vector2 Scale;
    public Angle   Rotation;
  }
}