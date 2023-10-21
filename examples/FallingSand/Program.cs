Game.Start(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Falling Sand",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080
    }
  },
  Host = GameHost.Create(() =>
  {
    return _ =>
    {
      // TODO: implement me
    };
  })
});
