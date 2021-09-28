using Surreal;
using Surreal.Platform;

namespace Asteroids
{
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
          ShowFPSInTitle = true
        }
      }
    });
  }
}
