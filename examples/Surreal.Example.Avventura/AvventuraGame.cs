using Surreal.Input.Keyboard;

namespace Avventura;

public sealed class AvventuraGame : PrototypeGame
{
  public static Task Main() => StartAsync<AvventuraGame>(new Configuration
  {
    Platform = new ConsolePlatform
    {
      Configuration =
      {
        Title          = "Avventura",
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
