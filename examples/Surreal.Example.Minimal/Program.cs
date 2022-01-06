using Surreal.Fibers;
using Surreal.Graphics;
using Surreal.Mathematics;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
  },
};

Game.Start(platform, host =>
{
  var graphics = host.Services.GetRequiredService<IGraphicsDevice>();

  var sourceColor = Random.Shared.NextColor();
  var targetColor = Random.Shared.NextColor();

  return async context =>
  {
    var amount = MathF.Sin((float) context.GameTime.TotalTime.TotalSeconds);
    var color  = Color.Lerp(sourceColor, targetColor, amount);

    graphics.Clear(color);
    graphics.Present();

    await FiberTask.Yield();
  };
});
