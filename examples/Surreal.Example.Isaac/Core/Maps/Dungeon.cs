using Isaac.Core.Maps.Planning;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Maps {
  public sealed class Dungeon : Actor {
    public TileMap   TileMap   { get; } = new TileMap(256, 144);
    public FloorPlan FloorPlan { get; } = new FloorPlan();

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      FloorPlan.DrawGizmos(Game.Current.GeometryBatch);
    }
  }
}