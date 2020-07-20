using Isaac.Core.Dungeons;
using Isaac.Screens;
using Surreal;
using Surreal.Mathematics;
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

    public GameState State { get; } = new GameState();

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new DungeonScreen(
          game: this,
          generator: DungeonGenerators.Standard(Range.Of(6, 12))
      ));
    }
  }
}