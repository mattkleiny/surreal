using System.Collections.Generic;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;
using Surreal.Mathematics;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Surreal.Framework.PathFinding {
  public static class PathFindingExtensions {
    private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler(nameof(PathFindingExtensions));

    private const int MaxNeighbourCount = 16;
    private const int MaxPathSteps      = 100;

    public static Path FindPath<T, TNeighbourhood>(this IGrid<T> grid, Vector2I start, Vector2I end, Heuristic heuristic)
        where TNeighbourhood : struct, INeighbourhood {
      // distribute points uniformly and generically on the source grid, given a valid neighbourhood class for point comparison
      var pathfindingGrid = new UniformPathfindingGrid<T, TNeighbourhood>(grid);

      return FindPath(pathfindingGrid, start, end, heuristic);
    }

    public static Path FindPath<TGrid>(this TGrid grid, Vector2I start, Vector2I end, Heuristic heuristic)
        where TGrid : IPathFindingGrid {
      using var _ = Profiler.Track(nameof(FindPath));

      var frontier  = new PriorityQueue<Vector2I>();
      var cameFrom  = new Dictionary<Vector2I, Vector2I>();
      var costSoFar = new Dictionary<Vector2I, float>();

      cameFrom[start]  = start;
      costSoFar[start] = 0f;

      frontier.Enqueue(start, 0f);

      var neighbours = new SpanList<Vector2I>(stackalloc Vector2I[MaxNeighbourCount]);

      while (frontier.Count > 0 && costSoFar.Count < MaxPathSteps) {
        var current = frontier.Dequeue();
        if (current == end) {
          return RetracePath(start, end, cameFrom);
        }

        neighbours.Clear();
        grid.GetNeighbours(start, ref neighbours);

        for (var i = 0; i < neighbours.Count; i++) {
          var neighbour = neighbours[i];
          var newCost   = costSoFar[current] + grid.GetCost(current, neighbour);

          if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour]) {
            if (costSoFar.ContainsKey(neighbour)) {
              costSoFar.Remove(neighbour);
              cameFrom.Remove(neighbour);
            }

            costSoFar[neighbour] = newCost;
            cameFrom[neighbour]  = current;

            var priority = newCost + heuristic(neighbour, end);

            frontier.Enqueue(neighbour, priority);
          }
        }
      }

      return default;
    }

    private static Path RetracePath(
        Vector2I start,
        Vector2I end,
        IReadOnlyDictionary<Vector2I, Vector2I> cameFrom) {
      var points  = new SpanList<Vector2I>(stackalloc Vector2I[cameFrom.Count]);
      var current = end;

      while (current != start) {
        points.Add(current);

        if (current == start) {
          break;
        }

        current = cameFrom[current];
      }

      points.Add(start);

      // TODO: reverse the points

      return new Path(points.ToArray());
    }
  }
}