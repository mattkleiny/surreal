using RayCaster.Graphics;
using Viewport = Surreal.Graphics.PixelCanvas;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Ray Caster",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    Width          = 320 * 6,
    Height         = 200 * 6
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  using var shader = await game.Assets.LoadDefaultSpriteShaderAsync();
  using var canvas = new PixelCanvas(graphics, 320, 200);

  var map = TileMap.CreateDefault();

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    canvas.Pixels.Fill(Color32.Black);

    map.Draw(canvas.Pixels);
    canvas.Draw(shader);
  });
});
