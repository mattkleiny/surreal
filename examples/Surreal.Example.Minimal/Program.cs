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
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  var color1 = Random.Shared.NextColor();
  var color2 = Random.Shared.NextColor();

  await context.ExecuteAsync(time =>
  {
    var t     = MathF.Sin((float) time.TotalTime.TotalSeconds);
    var color = Color.Lerp(color1, color2, t);

    graphics.ClearColorBuffer(color);

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }
  });
});
