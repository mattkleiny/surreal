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

await Game.Start(platform, context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  var random = Random.Shared;

  var color1 = random.NextColor();
  var color2 = random.NextColor();

  context.Execute(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    var t = MathF.Sin((float) time.TotalTime.TotalSeconds);
    var lerp = Color.Lerp(color1, color2, t);

    graphics.ClearColorBuffer(lerp);
  });

  return ValueTask.CompletedTask;
});
