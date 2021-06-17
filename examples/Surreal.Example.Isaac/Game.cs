using System.IO;
using System.Text;
using System.Threading.Tasks;
using Isaac.Screens;
using Surreal;
using Surreal.Diagnostics.Console.Interpreter;
using Surreal.Diagnostics.Logging;
using Surreal.IO;
using Surreal.IO.Serialization;
using Surreal.Platform;
using Path = Surreal.IO.Path;

namespace Isaac {
  public sealed class Game : GameJam<Game> {
    private static readonly ILog Log = LogFactory.GetLog<Game>();

    public static void Main() => Start<Game>(new() {
      Platform = new DesktopPlatform {
        Configuration = {
          Title          = "The Binding of Isaac",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });

    public GameState State { get; private set; } = new();

    protected override void Initialize() {
      base.Initialize();

      GraphicsDevice.Pipeline.Rasterizer.IsBlendingEnabled = true;

      Screens.Push(new MainScreen(this));
    }

    protected override void RegisterConsoleBindings(IConsoleBindings bindings) {
      base.RegisterConsoleBindings(bindings);

      bindings.Add("save", () => SaveAsync("./quicksave.sav").Wait());
      bindings.Add("load", () => LoadAsync("./quicksave.sav").Wait());
    }

    public Task SaveAsync(Path path) {
      return Log.ProfileAsync($"Saving game to {path}", async () => {
        await using var stream = await path.OpenOutputStreamAsync();
        await using var writer = new BinaryWriter(stream, Encoding.UTF8);

        writer.WriteBinaryObject(State);
      });
    }

    public Task LoadAsync(Path path) {
      return Log.ProfileAsync($"Loading game from {path}", async () => {
        await using var stream = await path.OpenInputStreamAsync();
        using var       reader = new BinaryReader(stream, Encoding.UTF8);

        State = reader.ReadBinaryObject<GameState>();
      });
    }
  }
}