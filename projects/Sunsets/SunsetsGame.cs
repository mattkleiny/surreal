using Sunsets.Screens;
using Surreal;
using Surreal.Platform;

namespace Sunsets {
  public sealed class SunsetsGame : GameJam<SunsetsGame> {
    public static void Main() => Start<SunsetsGame>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "Sunsets",
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