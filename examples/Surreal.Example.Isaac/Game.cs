using Isaac.Dungeons;
using Surreal.Assets;
using Surreal.Objects.Templates;

namespace Isaac;

public sealed class Game : PrototypeGame
{
  private DungeonBlueprint.Template blueprint;

  public static Task Main() => StartAsync<Game>(new Configuration
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

  protected override async Task LoadContentAsync(IAssetContext assets)
  {
    blueprint = await assets.LoadAsset<DungeonBlueprint.Template>("Assets/dungeons/dungeon-test-01.xml");
  }

  protected override void RegisterAssetLoaders(IAssetManager assets)
  {
    base.RegisterAssetLoaders(assets);

    assets.AddLoader(new XmlTemplateLoader<DungeonBlueprint.Template>("Dungeon"));
  }
}
