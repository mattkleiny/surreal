using Surreal.Terminals;

var platform = new ConsolePlatform();

await Game.StartAsync(platform, async context =>
{
  var color1 = Random.Shared.NextColor();
  var color2 = Random.Shared.NextColor();

  var terminal = new ConsoleTerminal();
  var random = new Random();

  var glyph = new Glyph('X', color1, color2);

  await context.ExecuteAsync(_ =>
  {
    for (int i = 0; i < 16; i++)
    {
      var x = random.Next(0, terminal.Width);
      var y = random.Next(0, terminal.Height);

      terminal.DrawGlyph(x, y, glyph);
    }
  });
});
