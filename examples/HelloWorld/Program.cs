var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Hello, Surreal!",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1024,
      Height = 768
    }
  }
};

return Game.Start(configuration, (Game game, IGraphicsDevice graphics, IKeyboardDevice keyboard) =>
{
  var color1 = Random.Shared.Next<Color>();
  var color2 = Random.Shared.Next<Color>();
  var timer = 0f;

  game.Update += time =>
  {
    timer += time.DeltaTime;

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  };

  game.Render += _ =>
  {
    var color = Color.Lerp(color1, color2, MathE.PingPong(timer));

    graphics.ClearColorBuffer(color);
  };

  game.ExecuteVariableStep();
});
