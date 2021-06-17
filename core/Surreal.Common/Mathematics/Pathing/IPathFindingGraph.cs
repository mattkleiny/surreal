using System.Collections.Generic;
using Surreal.Collections;

namespace Surreal.Mathematics.Pathing {
  public interface IPathFindingGraph<TNode>
      where TNode : class {
    float              GetCost(TNode from, TNode to);
    IEnumerable<TNode> GetConnections(TNode node);
  }

  public static class PathFindingGraphExtensions {
    public static Path<TNode> FindPath<TNode>(
        this IPathFindingGraph<TNode> graph,
        TNode start,
        TNode goal,
        PathHeuristic<TNode>? heuristic = default,
        int maximumSteps = 512
    ) where TNode : class {
      var frontier  = new BinaryHeap<TNode, float>(Comparers.FloatAscending);
      var cameFrom  = new Dictionary<TNode, TNode>();
      var costSoFar = new Dictionary<TNode, float>();

      cameFrom[start]  = start;
      costSoFar[start] = 0f;

      frontier.Push(start, 0f);

      while (frontier.Count > 0 && costSoFar.Count < maximumSteps) {
        var current = frontier.Pop();
        if (current == goal) {
          return RetracePath(start, goal, cameFrom);
        }

        foreach (var neighbour in graph.GetConnections(current)) {
          var newCost = costSoFar[current] + graph.GetCost(current, neighbour);

          if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour]) {
            if (costSoFar.ContainsKey(neighbour)) {
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

      return Path<TNode>.Empty;
    }

    private static Path<TNode> RetracePath<TNode>(
        TNode start,
        TNode goal,
        IReadOnlyDictionary<TNode, TNode> cameFrom
    ) where TNode : class {
      var path    = new List<TNode>(cameFrom.Count);
      var current = goal;

      while (current != start) {
        path.Add(current);

        if (current == start) {
          break;
        }

        current = cameFrom[current];
      }

      path.Add(start);
      path.Reverse();

      return new Path<TNode>(path);
    }
  }
}