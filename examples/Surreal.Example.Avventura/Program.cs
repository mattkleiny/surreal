var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Avventura",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    IconPath       = "resx://Avventura/Resources/icons/avventura.png"
  }
};

Game.Start(platform, game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var input = game.Services.GetRequiredService<IInputServer>();

  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    graphics.ClearColorBuffer(Color.White);
  });

  return Task.CompletedTask;
});
