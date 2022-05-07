var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Alchemist",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    IconPath       = "resx://Alchemist/Resources/icons/alchemist.png"
  }
};

Game.Start(platform, game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    graphics.ClearColorBuffer(Color.Black);
  });

  return Task.CompletedTask;
});
