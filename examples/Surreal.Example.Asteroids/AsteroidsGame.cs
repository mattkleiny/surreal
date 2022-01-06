using Asteroids.Screens;

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
}
