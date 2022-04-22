using Surreal.Input.Keyboard;

namespace Asteroids;

public sealed class AsteroidsGame : PrototypeGame
{
  public static void Main() => Start<AsteroidsGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title = "Asteroids",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override void OnInput(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    base.OnInput(time);
  }
}
