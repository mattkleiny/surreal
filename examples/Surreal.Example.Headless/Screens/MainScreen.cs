using Surreal.Framework.Screens;
using Surreal.Framework.Simulations;

namespace Headless.Screens {
  public sealed class MainScreen : SimulationScreen<HeadlessGame, ActorSimulation> {
    public MainScreen(HeadlessGame game)
        : base(game) {
    }

    protected override ActorSimulation CreateSimulation() {
      return new ActorSimulation();
    }
  }
}