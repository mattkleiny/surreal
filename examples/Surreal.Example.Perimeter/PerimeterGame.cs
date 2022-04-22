using Surreal.Input.Keyboard;

namespace Perimeter;

public sealed class PerimeterGame : PrototypeGame
{
  public static void Main() => Start<PerimeterGame>(new Configuration
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

  protected override void OnInput(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    base.OnInput(time);
  }
}
