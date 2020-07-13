using System.Numerics;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Graphics.Meshes;

namespace Mindustry.Modules.Core.Components {
  public struct GeometrySprite : IComponent {
    public Vector2[]     Points;
    public PrimitiveType Type;
  }
}