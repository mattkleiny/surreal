using Surreal.Input.Keyboard;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
  },
};

await Game.StartAsync(platform, async context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsDevice>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  var color1 = Random.Shared.NextColor();
  var color2 = Random.Shared.NextColor();

  await context.ExecuteAsync(time =>
  {
    var color = Color.Lerp(color1, color2, MathF.Sin((float) time.TotalTime.TotalSeconds));

    graphics.Clear(color);
    graphics.Present();

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }
  });
});
