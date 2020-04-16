using Surreal.Framework.Screens;
using Surreal.Framework.Simulations;

namespace Sunsets.Screens
{
  public sealed class MainScreen : SimulationScreen<SunsetsGame, EntitySimulation>
  {
    public MainScreen(SunsetsGame game)
      : base(game)
    {
    }

    protected override EntitySimulation CreateSimulation()
    {
      return new EntitySimulation();
    }
  }
}
