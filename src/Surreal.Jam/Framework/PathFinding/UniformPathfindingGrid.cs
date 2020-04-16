using Surreal.Collections;
using Surreal.Mathematics;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Surreal.Framework.PathFinding
{
  internal readonly struct UniformPathfindingGrid<T, TNeighbourhood> : IGrid<T>, IPathFindingGrid
    where TNeighbourhood : struct, INeighbourhood
  {
    private readonly IGrid<T> grid;
    private readonly float    cost;

    public UniformPathfindingGrid(IGrid<T> grid, float cost = 1f)
    {
      this.grid = grid;
      this.cost = cost;
    }

    public int Width  => grid.Width;
    public int Height => grid.Height;

    public T this[int x, int y]
    {
      get => grid[x, y];
      set => grid[x, y] = value;
    }

    public float GetCost(Vector2I from, Vector2I to)
    {
      return cost;
    }

    public void GetNeighbours(Vector2I position, ref SpanList<Vector2I> results)
    {
      foreach (var neighbour in position.GetNeighbourhood<TNeighbourhood>())
      {
        if (neighbour != position && grid.Contains(position.X, position.Y))
        {
          results.Add(neighbour);
        }
      }
    }
  }
}