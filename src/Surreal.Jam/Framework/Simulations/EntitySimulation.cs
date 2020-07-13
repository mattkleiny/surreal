using Surreal.Framework.Scenes.Entities;
using Surreal.Framework.Scenes.Entities.Systems;

namespace Surreal.Framework.Simulations {
  public class EntitySimulation : SceneSimulation<EntityScene> {
    public EntitySimulation()
        : base(new EntityScene()) {
    }

    public override void Initialize() {
      base.Initialize();

      Scene.AddSystem(new EventSystem(Events));
      Scene.Initialize();
    }
  }
}