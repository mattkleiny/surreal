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
  public sealed class MainScreen : GameScreen<Game> {
    public MainScreen(Game game)
        : base(game) {
    }

    public DungeonGenerator Generator { get; } = DungeonGenerators.Standard(Range.Of(6, 16));
    public ActorScene       Scene     { get; } = new ActorScene();
    public CameraRig        CameraRig { get; } = new CameraRig();

    public override void Initialize() {
      base.Initialize();

      Plugins.Add(new ScenePlugin(Scene));

      Scene.Actors.Add(CameraRig);
      Scene.Actors.Add(Generator(Game.State.Seed));
      Scene.Actors.Add(new Player());
      Scene.Actors.Add(new Monster(new ShooterBrain()));
      Scene.Actors.Add(new Monster(new ChargerBrain()));
    }

    public override async void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      if (Keyboard.IsKeyPressed(Key.F5)) {
        await Game.State.SaveAsync("./quicksave.sav");
      }

      if (Keyboard.IsKeyPressed(Key.F7)) {
        Game.State = await GameState.LoadAsync("./quicksave.sav");
      }

      base.Input(time);
    }

    public override void Draw(GameTime time) {
      GeometryBatch.Begin(in CameraRig.Camera.ProjectionView);

      base.Draw(time);

      GeometryBatch.End();
    }
  }
}