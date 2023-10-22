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
  Host = GameHost.Create(() =>
  {
    var graphics = Game.Services.GetServiceOrThrow<IGraphicsBackend>();
    var keyboard = Game.Services.GetServiceOrThrow<IKeyboardDevice>();

    var canvas = new PixelCanvas(graphics, 256, 144);

    return _ =>
    {
      graphics.ClearColorBuffer(new Color(0.2f, 0.2f, 0.2f, 0.8f));

      for (var y = 0; y < canvas.Height; y++)
      for (var x = 0; x < canvas.Width; x++)
      {
        canvas[x, y] = Random.Shared.Next<Color>();
      }

      canvas.DrawFullscreenQuad();

      if (keyboard.IsKeyPressed(Key.Escape))
      {
        Game.Exit();
      }
    };
  })
});
