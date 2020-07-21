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

    // persistent game state, with support for serialization to/from disk
    public GameState State { get; set; } = new GameState();

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));
    }
  }
}