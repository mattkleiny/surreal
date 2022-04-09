using Surreal.Input.Keyboard;
using Surreal.Terminals;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
  },
};

await Game.StartAsync(platform, async context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  var random = Random.Shared;

  var color1 = random.NextColor();
  var color2 = random.NextColor();

  var terminal = new GraphicsTerminal(graphics, 256, 256);
  var glyph = new Glyph('X', color1, color2);

  await context.ExecuteAsync(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    for (int i = 0; i < 16; i++)
    {
      var x = random.Next(0, terminal.Width);
      var y = random.Next(0, terminal.Height);

      terminal.DrawGlyph(x, y, glyph);
    }

    terminal.Flush();
  });
});
