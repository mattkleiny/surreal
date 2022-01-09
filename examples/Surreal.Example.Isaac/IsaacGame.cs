using Surreal.Input.Keyboard;
using Surreal.Windows;

namespace Isaac;

public sealed class IsaacGame : PrototypeGame
{
  public static Task Main() => GameEditor.StartAsync<IsaacGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "The Binding of Isaac",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    if (Keyboard.IsKeyPressed(Key.F8))
    {
      // TODO: implement me
      GameEditor.ShowWindow(new ObjectInspectorWindow());
    }

    base.Input(time);
  }
}
