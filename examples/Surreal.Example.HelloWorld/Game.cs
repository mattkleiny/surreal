using HelloWorld.Screens;
using Surreal;
using Surreal.Framework;
using Surreal.Framework.Parameters;
using Surreal.Framework.Tweening;
using Surreal.Graphics;
using Surreal.Platform;

namespace HelloWorld {
  public sealed class Game : GameJam<Game> {
    public static void Main() => Start<Game>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "Hello, Surreal!",
                IsVsyncEnabled = true,
                ShowFPSInTitle = true
            }
        }
    });

    protected override void Initialize() {
      base.Initialize();

      Screens.Push(new MainScreen(this));

      FloatParameter parameter = new FloatParameter(0f);

      parameter.TweenOverTime(0f, 1f, TweenAnimation.Default);
    }

    protected override void Draw(GameTime time) {
      GraphicsDevice.Clear(Color.Black);

      base.Draw(time);
    }
  }
}