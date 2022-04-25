namespace Asteroids;

/// <summary>The entry point for the Asteroids game.</summary>
public sealed class AsteroidsGame : PrototypeGame<AsteroidsGame>
{
  public static void Main(string[] args) => Start(new DesktopPlatform
  {
    Configuration =
    {
      Title          = "Asteroids",
      Width          = 1920,
      Height         = 1080,
      IsVsyncEnabled = false,
    },
  });

  protected override void OnRender(GameTime time)
  {
    GraphicsServer.ClearColorBuffer(Color.White);
  }
}
