using Mindustry.Modules.Core.Model.Resources;
using Surreal.Framework.Scenes.Entities.Components;

namespace Mindustry.Modules.Core.Components
{
  public sealed class ResourceStore : IComponent
  {
    public ResourceInventory Inventory { get; } = new ResourceInventory();
  }
}