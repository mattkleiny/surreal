using System.Numerics;
using Isaac.Core;
using Isaac.Core.Mobs;
using Surreal.Framework;
using Surreal.Framework.Scenes;
using Surreal.Framework.Scenes.Actors;
using Surreal.Framework.Screens;
using Surreal.Input.Keyboard;

namespace Isaac.Screens {
  public sealed class MainScreen : GameScreen<Game> {
    public MainScreen(Game game)
        : base(game) {
    }

    public ActorScene Scene     { get; } = new ActorScene();
    public CameraRig  CameraRig { get; } = new CameraRig();

    public override void Initialize() {
      base.Initialize();

      Plugins.Add(new ScenePlugin(Scene));

      Scene.Actors.Add(CameraRig);
      Scene.Actors.Add(new Dungeon());
      Scene.Actors.Add(new Player());
      Scene.Actors.Add(new Monster {Position = Vector2.UnitX * 1});
      Scene.Actors.Add(new Monster {Position = Vector2.UnitY * 2});
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