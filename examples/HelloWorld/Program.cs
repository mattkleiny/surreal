var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Hello, Surreal!",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080
    }
  }
};

Game.Start(configuration, game =>
{
  var graphics = game.Services.GetServiceOrThrow<IGraphicsBackend>();
  var keyboard = game.Services.GetServiceOrThrow<IKeyboardDevice>();

  var color1 = Random.Shared.Next<Color>();
  var color2 = Random.Shared.Next<Color>();

  game.ExecuteVariableStep(time =>
  {
    var color = Color.Lerp(color1, color2, MathE.PingPong(time.TotalTime));

    graphics.ClearColorBuffer(color);

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  });
});
