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

  var random = Random.Shared;

  var color1 = random.NextColor();
  var color2 = random.NextColor();

  context.ExecuteVariableStep(time =>
  {
    var blend = MathF.Sin(time.TotalTime);
    var color = Color.Lerp(color1, color2, blend);

    graphics.ClearColorBuffer(color);
  });

  return Task.CompletedTask;
});
