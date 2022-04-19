using Surreal.Input.Keyboard;
using Surreal.Terminals;

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
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  using var terminal = new ConsoleTerminal();

  var random = Random.Shared;

  var color1 = random.NextColor();
  var color2 = random.NextColor();

  var glyph = new Glyph('X', color1, color2);

  await context.ExecuteAsync(_ =>
  {
    if (keyboard.IsKeyDown(Key.Escape))
    {
      context.Exit();
    }

    for (int i = 0; i < 16; i++)
    {
      var x = random.Next(0, terminal.Width);
      var y = random.Next(0, terminal.Height);

      terminal.DrawGlyph(x, y, glyph);
    }
  });
});
