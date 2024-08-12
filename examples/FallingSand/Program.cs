using FallingSand;

var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Falling Sand",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080,
      IsTransparent = true
    }
  }
};

Game.Start(configuration, async (Game game, IGraphicsBackend graphics, IKeyboardDevice keyboard, IMouseDevice mouse) =>
{
  var palette = await game.Assets.LoadAssetAsync<ColorPalette>("resx://FallingSand/Assets/Embedded/palettes/kule-16.pal");

  using var canvas = new SandCanvas(graphics);

  game.ExecuteVariableStep(time =>
  {
    graphics.ClearColorBuffer(new Color(0.2f, 0.2f, 0.2f, 0.8f));

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
        canvas.DrawSand(relativePosition, 3, palette.SelectRandom());
      }
      else if (isRightButtonDown)
      {
        canvas.DrawSand(relativePosition, 3, Color32.Clear);
      }
    }

    canvas.Update(time.DeltaTime);
    canvas.DrawQuad();

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  });
});
