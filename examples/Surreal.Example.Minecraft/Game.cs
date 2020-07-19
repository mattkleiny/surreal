using Minecraft.Screens;
using Surreal;
using Surreal.Framework;
using Surreal.Graphics;
using Surreal.Platform;

namespace Minecraft {
  public sealed class Game : GameJam<Game> {
    public static void Main() => Start<Game>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "Minecraft",
                IsVsyncEnabled = true,
                ShowFPSInTitle = true
            }
        }
    });

    protected override void Initialize() {
      base.Initialize();

      Mouse.IsCursorVisible  = false;
      Mouse.IsLockedToWindow = true;

      GraphicsDevice.Pipeline.Rasterizer.IsDepthTestingEnabled = true;

      Screens.Push(new MainScreen(this).LoadAsync());
    }

    protected override void Draw(GameTime time) {
      GraphicsDevice.Clear(Color.Black);

      base.Draw(time);
    }
  }
}