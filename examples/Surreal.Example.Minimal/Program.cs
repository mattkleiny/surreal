var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
  },
};

Game.Start(platform, services =>
{
  var graphics = services.GetRequiredService<IGraphicsDevice>();

  var sourceColor = Random.Shared.NextColor();
  var targetColor = Random.Shared.NextColor();

  return context =>
  {
    var amount = MathF.Sin((float) context.GameTime.TotalTime.TotalSeconds);
    var color  = Color.Lerp(sourceColor, targetColor, amount);

    graphics.Clear(color);
    graphics.Present();

    return FiberTask.CompletedTask;
  };
});
