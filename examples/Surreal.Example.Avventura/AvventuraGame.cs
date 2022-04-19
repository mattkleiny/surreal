using Surreal.Input.Keyboard;

namespace Avventura;

public sealed class AvventuraGame : PrototypeGame
{
  public static Task Main() => StartAsync<AvventuraGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Avventura",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    base.Input(time);
  }
}
