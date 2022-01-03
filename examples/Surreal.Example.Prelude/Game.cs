namespace Prelude;

public sealed class Game : PrototypeGame
{
  public static Task Main() => StartAsync<Game>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Prelude of the Chambered",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });
}
