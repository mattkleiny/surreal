Game.Start(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Hello, Surreal!",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080
    }
  },
  Host = GameHost.Create(() =>
  {
    var color1 = Random.Shared.Next<Color>();
    var color2 = Random.Shared.Next<Color>();

    return time =>
    {
      var context = GraphicsContext.Default;
      var color = Color.Lerp(color1, color2, MathE.PingPong(time.TotalTime));

      context.Backend.ClearColorBuffer(color);
    };
  })
});
