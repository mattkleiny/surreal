using Isaac.Core.Actors;
using Isaac.Core.Controllers;
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

    var player = Scene.Spawn(new Player());

    Scene.AddSystem(new KeyboardControlSystem(new Pawn(player), Keyboard));
  }

  protected override void BeginFrame(GameTime time)
  {
    base.BeginFrame(time);

    Scene.BeginFrame(time.DeltaTime);
  }

  protected override void Input(GameTime time)
  {
    base.Input(time);

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

    Scene.Input(time.DeltaTime);
  }

  protected override void Update(GameTime time)
  {
    base.Update(time);

    Scene.Update(time.DeltaTime);
  }

  protected override void Draw(GameTime time)
  {
    base.Draw(time);

    Scene.Draw(time.DeltaTime);
  }

  protected override void EndFrame(GameTime time)
  {
    base.EndFrame(time);

    Scene.EndFrame(time.DeltaTime);
  }

  public override void Dispose()
  {
    Scene.Dispose();

    base.Dispose();
  }
}
