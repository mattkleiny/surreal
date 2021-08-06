using Asteroids.Screens;
using Surreal;
using Surreal.Platform;

namespace Asteroids
{
  public sealed class Game : GameJam
  {
    public static Game Current { get; private set; } = null!;

    public static void Main() => Start<Game>(new()
    {
      Platform = new DesktopPlatform
      {
        Configuration =
        {
          Title          = "Asteroids",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });

    protected override void Initialize()
    {
      Current = this;

      base.Initialize();

      Screens.Push(new MainScreen(this));
    }
  }
}
