using Mindustry.Simulation;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics.Cameras;
using Surreal.Input.Keyboard;

namespace Mindustry.Screens
{
  public sealed class MainScreen : SimulationScreen<MindustryGame, GameSimulation>
  {
    private readonly OrthographicCamera camera = new OrthographicCamera(256, 144);

    public MainScreen(MindustryGame game)
      : base(game)
    {
    }

    protected override GameSimulation CreateSimulation()
    {
      return new GameSimulation(Game, camera);
    }

    public override void Input(GameTime time)
    {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      base.Input(time);
    }
  }
}