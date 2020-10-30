using Isaac.Core.Maps.Planning;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Maps {
  public sealed class Dungeon : Actor {
    public TileMap   TileMap   { get; } = new(width: 256, height: 144);
    public FloorPlan FloorPlan { get; } = new();

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      // TileMap.Draw(Game.Current.GeometryBatch, deltaTime);
      FloorPlan.DrawGizmos(Game.Current.GeometryBatch);
    }
  }
}