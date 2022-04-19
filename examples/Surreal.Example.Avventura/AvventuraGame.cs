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
        FontSize       = 14
      },
    },
  });

  public new IConsolePlatformHost Host => (IConsolePlatformHost)base.Host;

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    base.Input(time);
  }

  protected override void Draw(GameTime time)
  {
    base.Draw(time);

    Host.Fill(' ');
    Host.DrawGlyph(16, 16, '█', ConsoleColor.Yellow);
  }
}
