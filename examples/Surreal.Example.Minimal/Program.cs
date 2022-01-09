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

await Game.StartAsync(platform, services =>
{
  var graphics = services.GetRequiredService<IGraphicsDevice>();
  var keyboard = services.GetRequiredService<IKeyboardDevice>();

  var sourceColor = Random.Shared.NextColor();
  var targetColor = Random.Shared.NextColor();

  return (context, time) =>
  {
    var amount = MathF.Sin((float) time.TotalTime.TotalSeconds);
    var color  = Color.Lerp(sourceColor, targetColor, amount);

    graphics.Clear(color);
    graphics.Present();

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    return ValueTask.CompletedTask;
  };
});
