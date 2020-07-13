using Surreal.Framework.Screens.Plugins;
using Surreal.Framework.Simulations;

namespace Surreal.Framework.Screens {
  public abstract class SimulationScreen<TGame, TSimulation> : GameScreen<TGame>
      where TGame : GameJam
      where TSimulation : class, ISimulation {
    protected SimulationScreen(TGame game)
        : base(game) {
    }

    public TSimulation Simulation { get; private set; }

    protected abstract TSimulation CreateSimulation();

    public override void Initialize() {
      base.Initialize();

      Simulation = CreateSimulation();
      Simulation.Initialize();

      Plugins.Add(new SimulationPlugin(Simulation));
    }
  }
}