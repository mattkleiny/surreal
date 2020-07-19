using Isaac.Core;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics.Cameras;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Linear;

namespace Isaac.Screens {
  public sealed class MainScreen : GameScreen<Game> {
    private readonly OrthographicCamera camera = new OrthographicCamera(256, 144);

    public MainScreen(Game game)
        : base(game) {
    }

    public Floor Floor { get; } = new Floor();

    public override void Initialize() {
      base.Initialize();

      Floor[0, 0]  = new Room {NormalDoors = Directions.North | Directions.South};
      Floor[-1, 0] = new Room {NormalDoors = Directions.East, SecretDoors = Directions.North};
      Floor[0, 1]  = new Room {NormalDoors = Directions.West, SecretDoors = Directions.South};
      Floor[0, 2]  = new Room {NormalDoors = Directions.South};
      Floor[2, 2]  = new Room {NormalDoors = Directions.North};
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) {
        Game.Exit();
      }

      base.Input(time);
    }

    public override void Draw(GameTime time) {
      base.Draw(time);

      GeometryBatch.Begin(in camera.ProjectionView);

      Floor.DrawGizmos(GeometryBatch);

      GeometryBatch.End();
    }
  }
}