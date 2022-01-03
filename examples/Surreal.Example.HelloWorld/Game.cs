using Surreal.Assets;
using Surreal.Graphics.Textures;

namespace HelloWorld;

public sealed class Game : PrototypeGame
{
  private Asset<Texture> texture;

  public static Task Main() => StartAsync<Game>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Hello, Surreal!",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override Task LoadContentAsync(IAssetContext assets)
  {
    texture = assets.LoadAsset<Texture>("Assets/Textures/HelloWorld.png");

    return base.LoadContentAsync(assets);
  }
}
