using Isaac.Dungeons;
using Surreal.Assets;

namespace Isaac;

public sealed class IsaacGame : PrototypeGame
{
  private DungeonBlueprint blueprint;

  public static Task Main() => StartAsync<IsaacGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "The Binding of Isaac",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override async Task LoadContentAsync(IAssetManager assets)
  {
    await base.LoadContentAsync(assets);

    blueprint = await assets.LoadAsset<DungeonBlueprint>("Assets/dungeons/dungeon-test-01.xml");
  }
}
