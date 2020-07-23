using Isaac.Core.Maps.Planning;
using Surreal.Framework.PathFinding;
using Surreal.Mathematics.Linear;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Maps {
  public sealed class Dungeon : Actor {
    public TileMap   TileMap   { get; } = new TileMap(256, 144);
    public FloorPlan FloorPlan { get; } = new FloorPlan();

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      using var path = FloorPlan.FindPath(Vector2I.Zero, Vector2I.UnitY, Heuristics.Euclidean);

      // TODO: support arbitrary transforms for wireframe geometry?
      FloorPlan.DrawGizmos(Game.Current.GeometryBatch);
    }
  }
}