using Isaac.Dungeons;
using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.Input.Keyboard;

namespace Isaac;

public sealed class IsaacGame : PrototypeGame
{
  private DungeonBlueprint dungeonBlueprint;
  private Image            sprite01;

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

  protected override async Task LoadContentAsync(IAssetManager assets, CancellationToken cancellationToken = default)
  {
    await base.LoadContentAsync(assets, cancellationToken);

    dungeonBlueprint = await assets.LoadAssetAsync<DungeonBlueprint>("Assets/dungeons/dungeon-test-01.xml", cancellationToken);
    sprite01         = await assets.LoadAssetAsync<Image>("Assets/sprites/sprite-test-01.xml", cancellationToken);
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    base.Input(time);
  }
}
