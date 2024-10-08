using FallingSand;

using var game = Game.Create(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Falling Sand",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1024,
      Height = 768,
      IsTransparent = true
    }
  }
});

var graphics = game.Services.GetServiceOrThrow<IGraphicsDevice>();
var mouse = game.Services.GetServiceOrThrow<IMouseDevice>();
var keyboard = game.Services.GetServiceOrThrow<IKeyboardDevice>();

var palette = await game.Assets.LoadAsync<ColorPalette>("resx://FallingSand/Assets/Embedded/palettes/kule-16.pal");

using var canvas = new SandCanvas(graphics);

game.FixedTick += time =>
{
  var isLeftButtonDown = mouse.IsButtonDown(MouseButton.Left);
  var isRightButtonDown = mouse.IsButtonDown(MouseButton.Right);

  if (isLeftButtonDown || isRightButtonDown)
  {
    var relativePosition = new Point2(
      (int)(mouse.NormalisedPosition.X * canvas.Pixels.Width),
      (int)(mouse.NormalisedPosition.Y * canvas.Pixels.Height)
    );

    if (isLeftButtonDown)
    {
      canvas.DrawSand(relativePosition, 3, palette.Value.SelectRandom());
    }
    else if (isRightButtonDown)
    {
      canvas.DrawSand(relativePosition, 3, Color32.Clear);
    }
  }

  canvas.Update(time.DeltaTime);

  if (keyboard.IsKeyPressed(Key.Escape))
  {
    game.Exit();
  }
};

game.Render += _ =>
{
  graphics.ClearColorBuffer(new Color(0.2f, 0.2f, 0.2f, 0.8f));

  canvas.DrawQuad();
};

await game.RunAsync();
