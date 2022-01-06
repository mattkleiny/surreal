namespace Minecraft;

public sealed class MinecraftGame : PrototypeGame
{
  public static Task Main() => StartAsync<MinecraftGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Minecraft",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override void Initialize()
  {
    base.Initialize();

    Mouse.IsCursorVisible = false;
  }
}
