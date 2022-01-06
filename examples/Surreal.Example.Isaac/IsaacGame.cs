using Isaac.Dungeons;
using Surreal.Assets;
using Surreal.Objects;

namespace Isaac;

public sealed class IsaacGame : PrototypeGame
{
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

  protected override void RegisterAssetLoaders(IAssetManager assets)
  {
    base.RegisterAssetLoaders(assets);

    assets.AddLoader(new XmlTemplateLoader<DungeonBlueprint.Template>("Dungeon"));
  }
}
