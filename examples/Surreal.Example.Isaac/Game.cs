using Surreal;
using Surreal.Platform;

namespace Isaac
{
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
          ShowFPSInTitle = true
        }
      }
    });
  }
}
