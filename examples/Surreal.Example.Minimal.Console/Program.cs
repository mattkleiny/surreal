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

Game.Start(platform, context =>
{
  var graphics = context.Services.GetRequiredService<IConsoleGraphics>();

  context.ExecuteVariableStep(_ =>
  {
    var x = Random.Shared.Next(0, graphics.Width);
    var y = Random.Shared.Next(0, graphics.Height);
    var color = Random.Shared.NextEnum<ConsoleColor>();

    graphics.Draw(x, y, new Glyph('█', color));
  });

  return Task.CompletedTask;
});
