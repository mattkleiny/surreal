using Surreal.Controls;
using Surreal.Input.Keyboard;

namespace Avventura;

public sealed class AvventuraGame : PrototypeGame
{
  public static Task Main() => GameEditor.StartAsync<AvventuraGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title = "Avventura",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    if (Keyboard.IsKeyPressed(Key.F8))
    {
      GameEditor.ShowWindow(new GraphEditorWindow
      {
        ViewModel =
        {
          Nodes =
          {
            new GraphNodeViewModel { Location = new Vector2(100f, 200f) },
            new GraphNodeViewModel { Location = new Vector2(200f, 200f) },
            new GraphNodeViewModel { Location = new Vector2(300f, 200f) },
            new GraphNodeViewModel { Location = new Vector2(400f, 200f) },
          },
        }
      });
    }

    if (Keyboard.IsKeyPressed(Key.F9))
    {
      GameEditor.ShowWindow(new TileGridEditorWindow());
    }

    base.Input(time);
  }
}
