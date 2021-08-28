using Surreal.Fibers;
using Surreal.Mathematics.Linear;

namespace Surreal.Mechanics.Abstractions
{
  public interface IMovable
  {
    FiberTask MoveTo(Point2 position);
  }
}
