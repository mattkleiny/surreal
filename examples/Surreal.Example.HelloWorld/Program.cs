var color1 = Random.Shared.NextColorF();
var color2 = Random.Shared.NextColorF();

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
  Tick = (time, server) =>
  {
    var color = Color.Lerp(color1, color2, Maths.PingPong(time.TotalTime));

    server.Backend.ClearDepthBuffer();
  }
});
