﻿using Isaac.Core.Actors;
using Surreal.Controls;
using Surreal.Input.Keyboard;

namespace Isaac;

public sealed class IsaacGame : PrototypeGame
{
  public static Task Main() => GameEditor.StartAsync<IsaacGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title = "The Binding of Isaac",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  public ActorScene Scene { get; } = new();

  protected override void Initialize()
  {
    base.Initialize();

    Scene.Spawn(new Player());
  }

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
