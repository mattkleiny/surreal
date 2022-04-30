var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IInputServer>().GetRequiredDevice<IKeyboardDevice>();

  var color1 = Random.Shared.NextColor();
  var color2 = Random.Shared.NextColor();

  context.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    graphics.ClearColorBuffer(Color.Lerp(color1, color2, Maths.PingPong(time.TotalTime)));
  });

  return Task.CompletedTask;
});
