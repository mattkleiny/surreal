using Surreal;

namespace Prelude;

public sealed class Game : PrototypeGame
{
  public static void Main() => Start<Game>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Prelude of the Chambered",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true
      }
    }
  });
}
