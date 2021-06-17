using System;
using System.Numerics;

namespace Surreal.Mathematics.Pathing {
  public delegate float PathHeuristic<in T>(T candidate, T goal);

  public static class PathHeuristics {
    public static readonly PathHeuristic<Vector2> Constant = (_, _) => 1f;
    public static          PathHeuristic<Vector2> Taxicab   => throw new NotImplementedException();
    public static          PathHeuristic<Vector2> Euclidean => throw new NotImplementedException();

    public static PathHeuristic<TNode> Map<TNode>(
        this PathHeuristic<Vector2> heuristic,
        Func<TNode, Vector2> provider) {
      return (candidate, goal) => {
        var start = provider(candidate);
        var end   = provider(goal);

        return heuristic(start, end);
      };
    }
  }
}