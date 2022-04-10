﻿using Isaac.Core.Actors;
using Isaac.Core.Dungeons;
using Isaac.Core.Systems;
using Surreal.Graphics.Meshes;
using Surreal.Input.Keyboard;

namespace Isaac;

public sealed class IsaacGame : PrototypeGame
{
  private Player? player;
  private Dungeon? dungeon;
  private SpriteBatch? batch;

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

    player = Scene.Spawn(new Player());
    dungeon = Scene.Spawn(new Dungeon(DungeonBlueprint.Simple));

    batch = new SpriteBatch(GraphicsServer, spriteCount: (15 * 9) * 2 + 32);

    Scene.AddSystem(new KeyboardSystem(new Pawn(player), Keyboard));
    Scene.AddSystem(new PhysicsSystem());
    Scene.AddSystem(new SpriteSystem(batch));
    Scene.AddSystem(new TileMapSystem(batch));
  }

  protected override void BeginFrame(GameTime time)
  {
    base.BeginFrame(time);

    Scene.BeginFrame(time.DeltaTime);
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    base.Input(time);

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

    batch?.Dispose();

    base.Dispose();
  }
}
