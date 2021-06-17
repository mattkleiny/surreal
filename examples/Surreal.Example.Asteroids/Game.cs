using Surreal;
using Surreal.Platform;

namespace Asteroids {
  public sealed class Game : GameJam<Game> {
    public static void Main() => Start<Game>(new() {
      Platform = new DesktopPlatform {
        Configuration = {
          Title          = "Asteroids",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });
  }
}