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

  var color1 = Random.Shared.NextColor();
  var color2 = Random.Shared.NextColor();

  context.ExecuteVariableStep(time =>
  {
    var color = Color.Lerp(color1, color2, Maths.PingPong(time.TotalTime));

    graphics.ClearColorBuffer(color);
  });

  return Task.CompletedTask;
});
