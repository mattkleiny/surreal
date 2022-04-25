var platform = new ConsolePlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal",
    Width          = 280,
    Height         = 120,
    ShowFpsInTitle = true,
  },
};

Game.Start(platform, context =>
{
  var random = Random.Shared;
  var graphics = context.Services.GetRequiredService<IConsoleGraphics>();

  context.ExecuteVariableStep(_ =>
  {
    for (int i = 0; i < 16; i++)
    {
      var x = random.Next(0, graphics.Width);
      var y = random.Next(0, graphics.Height);
      var color = random.NextEnum<ConsoleColor>();

      graphics.Draw(x, y, new Glyph('█', color));
    }
  });

  return Task.CompletedTask;
});
