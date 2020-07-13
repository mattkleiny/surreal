using Avventura.Screens;
using Surreal;
using Surreal.Platform;

namespace Avventura {
  public sealed class AvventuraGame : GameJam<AvventuraGame> {
    public static void Main() => Start<AvventuraGame>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "Avventura",
                Width          = 1920,
                Height         = 1080,
                IsVsyncEnabled = true,
                ShowFPSInTitle = true
            }
        }
    });

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));
    }
  }
}