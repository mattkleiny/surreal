using Mindustry.Modules.Core.Components;
using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Framework.Scenes.Entities.Systems;

namespace Mindustry.Modules.Core.Systems
{
  public sealed class ConveyorSystem : ParallelIteratingSystem
  {
    public ConveyorSystem() 
      : base(Aspect.Of<Conveyor>())
    {
    }
  }
}