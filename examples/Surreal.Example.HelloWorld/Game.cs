using Surreal;
using Surreal.Platform;

namespace HelloWorld
{
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
          ShowFPSInTitle = true,
        },
      },
    });
  }
}
