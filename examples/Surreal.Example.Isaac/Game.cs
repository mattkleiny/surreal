using Surreal;
using Surreal.Platform;

namespace Isaac {
  public sealed class Game : GameJam<Game> {
    public static void Main() => Start<Game>(new() {
      Platform = new DesktopPlatform {
        Configuration = {
          Title          = "The Binding of Isaac",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });

    protected override void Initialize() {
      base.Initialize();

      GraphicsDevice.Pipeline.Rasterizer.IsBlendingEnabled = true;
    }
  }
}