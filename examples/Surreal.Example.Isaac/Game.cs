using Surreal;
using Surreal.Content;
using Surreal.Fibers;
using Surreal.Graphics.Fonts;
using Surreal.Platform;
using Surreal.Terminals;

namespace Isaac
{
  public sealed class Game : PrototypeGame
  {
    public static void Main() => Start<Game>(new()
    {
      Platform = new DesktopPlatform
      {
        Configuration =
        {
          Title          = "The Binding of Isaac",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });

    private Asset<BitmapFont> font;

    public Terminal Terminal { get; private set; } = null!;

    protected override async FiberTask LoadContentAsync(IAssetResolver assets)
    {
      font = await assets.LoadAsset<BitmapFont>("Assets/terminal8x8.png");
    }

    protected override void Initialize()
    {
      base.Initialize();

      Terminal = new Terminal(GraphicsDevice, font, new(100, 70), new(8, 8));
    }
  }
}
