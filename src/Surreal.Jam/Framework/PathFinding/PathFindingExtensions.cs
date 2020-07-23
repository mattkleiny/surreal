using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Surreal.Framework.PathFinding {
  public static class PathFindingExtensions {
    private static readonly IProfiler               Profiler        = ProfilerFactory.GetProfiler(nameof(PathFindingExtensions));
    private static readonly ThreadLocal<WorkingSet> LocalWorkingSet = new ThreadLocal<WorkingSet>(() => new WorkingSet());

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

      var workingSet = LocalWorkingSet.Value;

      workingSet.Clear();

      var frontier  = workingSet.Frontier;
      var cameFrom  = workingSet.CameFrom;
      var costSoFar = workingSet.Cost;

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

        for (var i = 0; i < neighbours.Length; i++) {
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

    private static Path RetracePath(Vector2I start, Vector2I end, Dictionary<Vector2I, Vector2I> cameFrom) {
      var buffer = MemoryPool<Vector2I>.Shared.Rent(cameFrom.Count);
      var points = new SpanList<Vector2I>(buffer.Memory.Span);

      var current = end;

      while (current != start) {
        points.Add(current);

        if (current == start) {
          break;
        }

        current = cameFrom[current];
      }

      points.Add(start);
      points.Span.Reverse();

      return new Path(points, buffer);
    }

    private readonly struct UniformPathfindingGrid<T, TNeighbourhood> : IGrid<T>, IPathFindingGrid
        where TNeighbourhood : struct, INeighbourhood {
      private readonly IGrid<T> grid;

      public UniformPathfindingGrid(IGrid<T> grid) {
        this.grid = grid;
      }

      public int Width  => grid.Width;
      public int Height => grid.Height;

      public T this[int x, int y] {
        get => grid[x, y];
        set => grid[x, y] = value;
      }

      public void GetNeighbours(Vector2I position, ref SpanList<Vector2I> results) {
        foreach (var neighbour in position.GetNeighbourhood<TNeighbourhood>()) {
          if (neighbour != position && grid.IsValid(position.X, position.Y)) {
            results.Add(neighbour);
          }
        }
      }
    }

    private sealed class WorkingSet {
      public PriorityQueue<Vector2I>        Frontier { get; } = new PriorityQueue<Vector2I>();
      public Dictionary<Vector2I, Vector2I> CameFrom { get; } = new Dictionary<Vector2I, Vector2I>();
      public Dictionary<Vector2I, float>    Cost     { get; } = new Dictionary<Vector2I, float>();

      public void Clear() {
        Frontier.Clear();
        CameFrom.Clear();
        Cost.Clear();
      }
    }
  }
}