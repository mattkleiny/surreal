using Surreal.Terminals;

var platform = new ConsolePlatform();

await Game.StartAsync(platform, async context =>
{
  var random = Random.Shared;

  var color1 = random.NextColor();
  var color2 = random.NextColor();

  var terminal = new ConsoleTerminal();

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
