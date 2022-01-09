namespace Homeworld;

public sealed class HomeworldGame : PrototypeGame
{
  public static Task Main() => StartAsync<HomeworldGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Homeworld",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });
}
