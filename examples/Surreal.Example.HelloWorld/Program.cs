var color1 = Random.Shared.Next<Color>();
var color2 = Random.Shared.Next<Color>();

Game.Start(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Hello, Surreal!",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 256 * 6,
      Height = 144 * 6
    }
  },
  Host = GameHost.Anonymous((time, graphics) =>
  {
    var color = Color.Lerp(color1, color2, MathE.PingPong(time.TotalTime));

    graphics.Backend.ClearColorBuffer(color);
  })
});
