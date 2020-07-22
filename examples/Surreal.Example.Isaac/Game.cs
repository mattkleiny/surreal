using System.IO;
using System.Text;
using System.Threading.Tasks;
using Isaac.Screens;
using Surreal;
using Surreal.Diagnostics.Logging;
using Surreal.IO;
using Surreal.Platform;
using Path = Surreal.IO.Path;

namespace Isaac {
  public sealed class Game : GameJam<Game> {
    private static readonly ILog Log = LogFactory.GetLog<Game>();

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

      Screens.Push(new MainScreen(this));
    }

    public async Task SaveAsync(Path path) {
      await Log.ProfileAsync($"Saving game to {path}", async () => {
        await using var stream = await path.OpenOutputStreamAsync();
        await using var writer = new BinaryWriter(stream, Encoding.UTF8);

        State.Save(writer);
      });
    }

    public async Task LoadAsync(Path path) {
      await Log.ProfileAsync($"Loading game from {path}", async () => {
        await using var stream = await path.OpenInputStreamAsync();
        using var       reader = new BinaryReader(stream, Encoding.UTF8);

        State.Load(reader);
      });
    }
  }
}