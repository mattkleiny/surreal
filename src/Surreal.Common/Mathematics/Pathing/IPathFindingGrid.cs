using System.Collections.Generic;
using System.Numerics;
using Surreal.Collections;
using Surreal.Mathematics.Linear;
using Surreal.Memory;

namespace Surreal.Mathematics.Pathing
{
  public interface IPathFindingGrid
  {
    float GetCost(Point2 from, Point2 to);
    void  GetNeighbours(Point2 position, ref SpanList<Point2> results);
  }

  public static class PathFindingGridExtensions
  {
    public static unsafe Path<Vector2> FindPath<TGrid>(
        this TGrid grid,
        Point2 start,
        Point2 goal,
        PathHeuristic<Vector2>? heuristic = default,
        Vector2 offset = default,
        int maximumSteps = 512
    ) where TGrid : IPathFindingGrid
    {
      var frontier  = new BinaryHeap<Point2, float>(Comparers.FloatAscending);
      var cameFrom  = new Dictionary<Point2, Point2>();
      var costSoFar = new Dictionary<Point2, float>();

      cameFrom[start]  = start;
      costSoFar[start] = 0f;

      frontier.Push(start, 0f);

      // at most 9 neighbouring cells per expansion
      var results = new SpanList<Point2>(stackalloc Point2[9]);

      while (frontier.Count > 0 && costSoFar.Count < maximumSteps)
      {
        var current = frontier.Pop();
        if (current == goal)
        {
          return RetracePath(start, goal, cameFrom, offset);
        }

        results.Clear();
        grid.GetNeighbours(current, ref results);

        for (var i = 0; i < results.Count; i++)
        {
          var neighbour = results[i];
          var newCost   = costSoFar[current] + grid.GetCost(current, neighbour);

          if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
          {
            if (costSoFar.ContainsKey(neighbour))
            {
              costSoFar.Remove(neighbour);
              cameFrom.Remove(neighbour);
            }

            costSoFar[neighbour] = newCost;
            cameFrom[neighbour]  = current;

            var priority = newCost + heuristic?.Invoke(neighbour, goal) ?? 0f;

            frontier.Push(neighbour, priority);
          }
        }
      }

      return Path<Vector2>.Empty;
    }

    private static Path<Vector2> RetracePath(
        Point2 start,
        Point2 goal,
        IReadOnlyDictionary<Point2, Point2> cameFrom,
        Vector2 offset
    )
    {
      var path    = new List<Vector2>(cameFrom.Count);
      var current = goal;

      while (current != start)
      {
        path.Add(current + offset);

        if (current == start)
        {
          break;
        }

        current = cameFrom[current];
      }

      path.Add(start + offset);
      path.Reverse();

      return new Path<Vector2>(path);
    }
  }
}