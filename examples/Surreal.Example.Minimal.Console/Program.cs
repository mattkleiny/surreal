var platform = new ConsolePlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal",
    Width          = 280,
    Height         = 120,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IConsoleGraphics>();

  game.ExecuteVariableStep(_ =>
  {
    var x = Random.Shared.Next(0, graphics.Width);
    var y = Random.Shared.Next(0, graphics.Height);
    var color = Random.Shared.NextEnum<ConsoleColor>();

    graphics.Draw(x, y, new Glyph('█', color));
  });

  return Task.CompletedTask;
});
