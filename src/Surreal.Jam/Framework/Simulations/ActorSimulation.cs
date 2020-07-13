using Surreal.Framework.Scenes.Actors;

namespace Surreal.Framework.Simulations {
  public class ActorSimulation : SceneSimulation<ActorScene> {
    public ActorSimulation()
        : base(new ActorScene()) {
    }
  }
}