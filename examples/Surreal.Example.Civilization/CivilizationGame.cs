using Civilization.Core;
using Surreal.Assets;
using Surreal.Input.Keyboard;

namespace Civilization;

public sealed class CivilizationGame : PrototypeGame
{
  public static Task Main() => StartAsync<CivilizationGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title = "Civilization",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override void RegisterAssetLoaders(IAssetManager manager)
  {
    base.RegisterAssetLoaders(manager);

    manager.AddLoader(new RuleSetLoader());
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    base.Input(time);
  }
}
