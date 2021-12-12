using Surreal;

namespace Isaac;

public sealed class Game : PrototypeGame
{
  public static void Main() => Start<Game>(new()
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "The Binding of Isaac",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true
      }
    }
  });
}
