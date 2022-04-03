using Surreal.Input.Keyboard;

namespace Perimeter;

public sealed class PerimeterGame : PrototypeGame
{
  public static async Task Main() => await StartAsync<PerimeterGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Perimeter",
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
