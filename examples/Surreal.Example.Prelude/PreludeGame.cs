using Surreal.Input.Keyboard;

namespace Prelude;

public sealed class PreludeGame : PrototypeGame
{
  public static Task Main() => StartAsync<PreludeGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Prelude of the Chambered",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    base.Input(time);
  }
}
