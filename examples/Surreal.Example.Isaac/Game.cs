using Isaac.Screens;
using Surreal;
using Surreal.Platform;

namespace Isaac {
  public sealed class Game : GameJam<Game> {
    public static void Main() => Start<Game>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "The Binding of Isaac",
                IsVsyncEnabled = true,
                ShowFPSInTitle = true,
            }
        }
    });

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));
    }
  }
}