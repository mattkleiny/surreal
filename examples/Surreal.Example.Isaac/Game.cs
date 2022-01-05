using Isaac.Dungeons;
using Surreal.Assets;
using Surreal.Objects;

namespace Isaac;

public sealed class Game : PrototypeGame
{
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

  protected override void RegisterAssetLoaders(IAssetManager assets)
  {
    base.RegisterAssetLoaders(assets);

    assets.AddLoader(new XmlTemplateLoader<DungeonBlueprint.Template>("Dungeon"));
  }
}
