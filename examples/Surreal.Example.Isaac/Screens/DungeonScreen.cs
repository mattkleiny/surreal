using System.Numerics;
using Isaac.Core;
using Isaac.Core.Brains;
using Isaac.Core.Dungeons;
using Isaac.Core.Mobs;
using Surreal.Framework;
using Surreal.Framework.Scenes;
using Surreal.Framework.Scenes.Actors;
using Surreal.Framework.Screens;
using Surreal.Input.Keyboard;
using Surreal.Mathematics;

namespace Isaac.Screens {
  public sealed class DungeonScreen : GameScreen<Game> {
    public DungeonScreen(Game game, DungeonGenerator generator)
        : base(game) {
      Generator = generator;
    }

    public DungeonGenerator Generator { get; }
    public ActorScene       Scene     { get; } = new ActorScene();
    public CameraRig        CameraRig { get; } = new CameraRig();

    public override void Initialize() {
      base.Initialize();

      Plugins.Add(new ScenePlugin(Scene));

      Restart(Game.State.Seed);
    }

    private void Restart(Seed seed = default) {
      Scene.Actors.Clear();

      Scene.Actors.Add(CameraRig);
      Scene.Actors.Add(Generator(seed));
      Scene.Actors.Add(new Player());
      Scene.Actors.Add(new Monster(new ShooterBrain()) {Position = Vector2.UnitX * 1});
      Scene.Actors.Add(new Monster(new ChargerBrain()) {Position = Vector2.UnitY * 2});
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      base.Input(time);
    }

    public override void Draw(GameTime time) {
      GeometryBatch.Begin(in CameraRig.Camera.ProjectionView);

      base.Draw(time);

      GeometryBatch.End();
    }
  }
}