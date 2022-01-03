using Surreal.Fibers;
using Surreal.Graphics;
using Surreal.Graphics.Sprites;
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
  var graphics    = host.Services.GetRequiredService<IGraphicsDevice>();
  var targetColor = Random.Shared.NextColor();

  using var spriteBatch = new SpriteBatch(graphics, spriteCount: 32);

  return async context =>
  {
    var color = Color.Lerp(Color.White, targetColor, Maths.PingPong((float) context.GameTime.TotalTime.TotalSeconds, 1));

    graphics.Clear(color);
    graphics.Present();

    await FiberTask.Yield();
  };
});
