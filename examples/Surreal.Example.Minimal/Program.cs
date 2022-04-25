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

Game.Start(platform, context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  var random = Random.Shared;

  var color1 = random.NextColor();
  var color2 = random.NextColor();

  context.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    var blend = MathF.Sin((float) time.TotalTime.TotalSeconds);
    var color = Color.Lerp(color1, color2, blend);

    graphics.ClearColorBuffer(color);
  });

  return Task.CompletedTask;
});
