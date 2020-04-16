using System;
using Surreal.Mathematics.Linear;

namespace Surreal.Framework.PathFinding
{
  public delegate float Heuristic(Vector2I start, Vector2I end);

  public static class Heuristics
  {
    public static readonly Heuristic Identity  = (start, goal) => 1f;
    public static readonly Heuristic Euclidean = (start, goal) => MathF.Abs(start.X - goal.X) + MathF.Abs(start.Y - goal.Y);
    public static          Heuristic Constant(float value = 1f) => (start, goal) => value;
  }
}