using Surreal.Collections;
using Surreal.Mathematics.Linear;

namespace Surreal.Framework.PathFinding
{
  public interface IPathFindingGrid
  {
    float GetCost(Vector2I from, Vector2I to);
    void  GetNeighbours(Vector2I position, ref SpanList<Vector2I> results);
  }
}
