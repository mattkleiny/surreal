var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Minecraft",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
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
