using Surreal.Input.Keyboard;

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

await Game.Start(platform, context =>
{
  var display = context.Services.GetRequiredService<IConsoleDisplay>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  var random = Random.Shared;

  context.Execute(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      display.Fill(' ');
    }

    for (int i = 0; i < 16; i++)
    {
      var x = random.Next(0, display.Width);
      var y = random.Next(0, display.Height);
      var color = random.NextEnum<ConsoleColor>();

      display.Draw(x, y, new Glyph('█', color));
    }
  });

  return ValueTask.CompletedTask;
});
