using Surreal.Input.Keyboard;
using Surreal.Screens;

namespace Asteroids.Screens;

public sealed class MainScreen : Screen<AsteroidsGame>
{
  public MainScreen(AsteroidsGame game)
    : base(game)
  {
  }

  public override void Input(GameTime time)
  {
    if (Game.Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

    base.Input(time);
  }
}
