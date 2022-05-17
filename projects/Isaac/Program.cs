var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Isaac",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    IconPath       = "resx://Isaac/Resources/icons/isaac.png"
  }
};

Game.Start(platform, game =>
{
  // ReSharper disable AccessToDisposedClosure

  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  game.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    graphics.ClearColorBuffer(Color.Black);
  });

  return Task.CompletedTask;
});
