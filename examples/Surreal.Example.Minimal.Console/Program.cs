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

await Game.StartAsync(platform, async context =>
{
  var host = context.Services.GetRequiredService<IConsolePlatformHost>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  var random = Random.Shared;

  await context.ExecuteAsync(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      host.FillGlyph(' ');
    }

    for (int i = 0; i < 16; i++)
    {
      var x = random.Next(0, host.Width);
      var y = random.Next(0, host.Height);
      var color = random.NextEnum<ConsoleColor>();

      host.DrawGlyph(x, y, '█', color);
    }
  });
});
