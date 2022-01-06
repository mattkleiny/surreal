namespace Prelude;

public sealed class PreludeGame : PrototypeGame
{
  public static Task Main() => StartAsync<PreludeGame>(new Configuration
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
