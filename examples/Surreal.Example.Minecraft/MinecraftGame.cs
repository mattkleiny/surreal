using Surreal.Graphics.Images;
using Surreal.Input.Keyboard;

namespace Minecraft;

public sealed class MinecraftGame : PrototypeGame
{
  public static async Task Main() => await StartAsync<MinecraftGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Minecraft",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
        Icon           = await Image.LoadAsync("resx://Minecraft/Minecraft.png"),
      },
    },
  });

  protected override void Initialize()
  {
    base.Initialize();

    Mouse.IsCursorVisible = false;
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    base.Input(time);
  }
}
