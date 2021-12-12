using Surreal;

namespace Asteroids;

public sealed class Game : PrototypeGame
{
  public static void Main() => Start<Game>(new()
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Asteroids",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true
      }
    }
  });
}
