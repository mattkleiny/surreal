using Surreal;
using Surreal.Platform;

namespace Minecraft
{
  public sealed class Game : GameJam<Game>
  {
    public static void Main() => Start<Game>(new()
    {
      Platform = new DesktopPlatform
      {
        Configuration =
        {
          Title          = "Minecraft",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });

    protected override void Initialize()
    {
      base.Initialize();

      Mouse.IsCursorVisible  = false;
      Mouse.IsLockedToWindow = true;

      GraphicsDevice.Pipeline.Rasterizer.IsDepthTestingEnabled = true;
    }
  }
}