using Surreal;

namespace Minecraft;

public sealed class Game : PrototypeGame
{
  public static void Main() => Start<Game>(new()
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Minecraft",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true
      }
    }
  });

  protected override void Initialize()
  {
    base.Initialize();

    Mouse.IsCursorVisible = false;

    GraphicsDevice.Pipeline.Rasterizer.IsDepthTestingEnabled = true;
  }
}
