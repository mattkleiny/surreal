using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Sprites;

namespace Silence.Screens
{
  public static class LoadingScreens
  {
    public static LoadingScreen LoadAsync<TState>(this TState screen)
      where TState : GameScreen<SilenceGame>, ILoadableScreen
    {
      return new LoadingScreen(screen.Game, screen);
    }
  }

  public sealed class LoadingScreen : LoadingScreen<SilenceGame, ILoadableScreen>
  {
    private BitmapFont font;

    public LoadingScreen(SilenceGame game, ILoadableScreen screen)
      : base(game, screen)
    {
    }

    protected override async Task LoadContentAsync(IAssetResolver assets)
    {
      await base.LoadContentAsync(assets);

      font = await assets.LoadDefaultFontAsync();
    }

    public override void Draw(GameTime time)
    {
      base.Draw(time);

      var message = $"Loading {Progress}% complete";
      var (width, height) = font!.Measure(message);

      SpriteBatch.DrawText(
        text: message,
        font: font!,
        position: new Vector2(
          x: Game.Host.Width / 2f - width / 2f,
          y: Game.Host.Height / 2f - height / 2f
        ),
        color: Color.White
      );
    }
  }
}