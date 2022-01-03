namespace Isaac;

public sealed class Game : PrototypeGame
{
  public static Task Main() => StartAsync<Game>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "The Binding of Isaac",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });
}
