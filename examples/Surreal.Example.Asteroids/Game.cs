using System.Numerics;
using Asteroids.Screens;
using Surreal;
using Surreal.Framework;
using Surreal.Graphics;
using Surreal.Platform;

namespace Asteroids {
  public sealed class Game : GameJam<Game> {
    private static readonly Matrix4x4 ProjectionView = Matrix4x4.CreateOrthographic(1920f, 1080f, 0.1f, 300f);

    public static void Main() => Start<Game>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "Asteroids",
                IsVsyncEnabled = true,
                ShowFPSInTitle = true
            }
        }
    });

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));
    }

    protected override void Draw(GameTime time) {
      GraphicsDevice.Clear(Color.Black);
      SpriteBatch.Begin(in ProjectionView);

      base.Draw(time);

      SpriteBatch.End();
    }
  }
}