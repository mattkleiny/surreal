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
      Height = 1080
    }
  },
  Host = GameHost.Create(() =>
  {
    var graphics = Game.Services.GetServiceOrThrow<IGraphicsBackend>();
    var canvas = new PixelCanvas(graphics, 256, 144);

    return _ =>
    {
      graphics.ClearColorBuffer(Color.Black);

      canvas.DrawFullscreenQuad();
    };
  })
});
