var platform = new DesktopPlatform
{
  Configuration =
  {
    Title = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, game =>
{
  // ReSharper disable AccessToDisposedClosure

  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  var color1 = Random.Shared.NextColor();
  var color2 = Random.Shared.NextColor();

  game.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    graphics.ClearColorBuffer(Color.Lerp(color1, color2, Maths.PingPong(time.TotalTime)));
  });

  return Task.CompletedTask;
});
