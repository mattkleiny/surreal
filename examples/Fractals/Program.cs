Game.Start(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Fractals",
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

      canvas.DrawFullscreenQuad();

      if (keyboard.IsKeyPressed(Key.Escape))
      {
        Game.Exit();
      }
    };
  })
});
