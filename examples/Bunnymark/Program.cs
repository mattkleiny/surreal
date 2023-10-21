Game.Start(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Bunnymark",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080
    }
  },
  Host = GameHost.Create(() =>
  {
    using var batch = new SpriteBatch();

    return time =>
    {
      // TODO: draw some bunnies
    };
  })
});
