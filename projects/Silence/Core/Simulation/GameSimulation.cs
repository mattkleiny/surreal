using Surreal.Framework.Simulations;

namespace Silence.Core.Simulation
{
  public sealed class GameSimulation : ActorSimulation
  {
    public SilenceGame Game { get; }

    public GameSimulation(SilenceGame game)
    {
      Game = game;
    }

    public override void Initialize()
    {
      base.Initialize();
    }
  }
}
