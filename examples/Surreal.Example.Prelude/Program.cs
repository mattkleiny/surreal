const int width = 320;
const int height = 200;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    Width          = width * 6,
    Height         = height * 6
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  using var shader = await game.Assets.LoadDefaultSpriteShaderAsync();
  using var canvas = new PixelCanvas(graphics, width, height);

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    var pixels = canvas.Pixels;

    pixels.Fill(Color32.Black);
    pixels.DrawLine(new(0, 0), new(320, 200), Color32.White);

    canvas.Draw(shader);
  });
});
