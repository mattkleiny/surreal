using Isaac.Core.Dungeons;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics.Cameras;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Linear;

namespace Isaac.Screens {
  public sealed class MainScreen : GameScreen<Game> {
    public MainScreen(Game game)
        : base(game) {
    }

    public OrthographicCamera Camera { get; } = new OrthographicCamera(256 / 2, 144 / 2);
    public Floor              Floor  { get; } = new Floor();

    public override void Initialize() {
      base.Initialize();

      var room = Floor[0, 0] = new Room {
          Type = RoomType.Start
      };

      room.AddRoom(Direction.North)
          .AddRoom(Direction.North, RoomType.Item)
          .AddRoom(Direction.East)
          .AddRoom(Direction.East)
          .AddRoom(Direction.South, RoomType.Boss)
          .AddRoom(Direction.West, RoomType.Secret);
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) {
        Game.Exit();
      }

      base.Input(time);
    }

    public override void Draw(GameTime time) {
      base.Draw(time);

      GeometryBatch.Begin(in Camera.ProjectionView);

      Floor.DrawGizmos(GeometryBatch);

      GeometryBatch.End();
    }
  }
}