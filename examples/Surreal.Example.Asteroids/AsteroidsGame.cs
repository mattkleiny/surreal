using Asteroids.Screens;
using Surreal.Input.Keyboard;

namespace Asteroids;

public sealed class AsteroidsGame : PrototypeGame
{
  public static Task Main() => StartAsync<AsteroidsGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Asteroids",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override void Initialize()
  {
    base.Initialize();

    Screens.Push(new MainScreen(this));
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    base.Input(time);
  }
}
