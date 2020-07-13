using System;
using Surreal.Framework.Simulations;

namespace Surreal.Framework.Screens.Plugins {
  public class SimulationPlugin : ScreenPlugin, IDisposable {
    public ISimulation Simulation { get; }

    public SimulationPlugin(ISimulation simulation) {
      Simulation = simulation;
    }

    public override void Begin() {
      Simulation.Begin();

      base.Begin();
    }

    public override void Input(GameTime time) {
      base.Input(time);

      Simulation.Input(time.DeltaTime);
    }

    public override void Update(GameTime time) {
      base.Update(time);

      Simulation.Update(time.DeltaTime);
    }

    public override void Draw(GameTime time) {
      base.Draw(time);

      Simulation.Draw(time.DeltaTime);
    }

    public override void End() {
      base.End();

      Simulation.End();
    }

    public virtual void Dispose() {
      Simulation.Dispose();
    }
  }
}