using Surreal.Graphics.Images;
using Surreal.Input.Keyboard;

namespace Minecraft;

public sealed class MinecraftGame : PrototypeGame
{
  private static readonly ConcurrentQueue<Action> Actions = new();

  public static async Task Main() => await StartAsync<MinecraftGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title = "Minecraft",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
        Icon = await Image.LoadAsync("resx://Minecraft/Minecraft.png"),
      },
    },
  });

  /// <summary>Schedules an action to be performed at the start of the next frame/</summary>
  public static void Schedule(Action action)
  {
    Actions.Enqueue(action);
  }

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

  protected override void Update(GameTime time)
  {
    base.Update(time);

    while (Actions.TryDequeue(out var action))
    {
      action.Invoke();
    }
  }
}
