using FallingSand;

Game.Start(new GameConfiguration
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
  },
  Host = GameHost.Create(async () =>
  {
    var graphics = Game.Services.GetServiceOrThrow<IGraphicsBackend>();
    var keyboard = Game.Services.GetServiceOrThrow<IKeyboardDevice>();
    var mouse = Game.Services.GetServiceOrThrow<IMouseDevice>();

    var canvas = new SandCanvas(graphics);

    var palette = await Game.Assets.LoadAssetAsync<ColorPalette>("resx://FallingSand/Assets/Embedded/palettes/kule-16.pal");

    return time =>
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
        Game.Exit();
      }
    };
  })
});
