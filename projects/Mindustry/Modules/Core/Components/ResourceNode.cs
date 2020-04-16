using Mindustry.Modules.Core.Model.Resources;
using Surreal.Framework.Scenes.Entities.Components;

namespace Mindustry.Modules.Core.Components
{
  public struct ResourceNode : IComponent
  {
    public ResourceStack Resources;

    public bool IsExhausted => Resources.Count == 0;
  }
}