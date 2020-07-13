using System.Numerics;
using Surreal.Framework.Scenes.Entities.Components;

namespace Mindustry.Modules.Core.Components {
  public struct Mobile : IComponent {
    public float   Speed;
    public Vector2 Direction;
  }
}