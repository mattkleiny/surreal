using Surreal.Input.Keyboard;

namespace Minecraft;

public sealed class MinecraftGame : PrototypeGame
{
  private static readonly ConcurrentQueue<Action> Actions = new();

  public static void Main() => Start<MinecraftGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Minecraft",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  /// <summary>Schedules an action to be performed at the start of the next frame/</summary>
  public static void Schedule(Action action)
  {
    Actions.Enqueue(action);
  }

  protected override void OnInitialize()
  {
    base.OnInitialize();

    Mouse.IsCursorVisible = false;
  }

  protected override void OnInput(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    base.OnInput(time);
  }

  protected override void OnUpdate(GameTime time)
  {
    base.OnUpdate(time);

    while (Actions.TryDequeue(out var action))
    {
      action.Invoke();
    }
  }
}
