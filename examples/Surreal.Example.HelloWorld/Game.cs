using HelloWorld.Screens;
using Surreal;
using Surreal.Platform;

namespace HelloWorld {
  public sealed class Game : GameJam<Game> {
    public static void Main() => Start<Game>(new() {
      Platform = new DesktopPlatform {
        Configuration = {
          Title          = "Hello, Surreal!",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));
    }
  }
}