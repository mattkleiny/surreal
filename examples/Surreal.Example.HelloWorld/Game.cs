using Surreal;

namespace HelloWorld;

public sealed class Game : PrototypeGame
{
  public static void Main() => Start<Game>(new()
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Hello, Surreal!",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true
      }
    }
  });
}
