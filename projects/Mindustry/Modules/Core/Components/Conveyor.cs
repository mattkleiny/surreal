using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Mathematics;

namespace Mindustry.Modules.Core.Components
{
  public struct Conveyor : IComponent
  {
    public Directions Connections;
    public float      Speed;
  }
}