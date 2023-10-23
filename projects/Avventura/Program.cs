﻿var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "The Last Voyage of the Avventura",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080,
      IconPath = "resx://Avventura/avventura.png"
    }
  }
};

Game.Start(configuration, (Game game, IGraphicsBackend graphics, IKeyboardDevice keyboard) =>
{
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
