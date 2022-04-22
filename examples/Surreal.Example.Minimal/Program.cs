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

await Game.Start(platform, async context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  var random = Random.Shared;

  var color1 = random.NextColor();
  var color2 = random.NextColor();

  await context.ExecuteAsync(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    graphics.ClearColorBuffer(Color.Lerp(color1, color2, MathF.Sin((float)time.TotalTime.TotalSeconds)));
  });
});
