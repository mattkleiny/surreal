using Surreal.Grids;
using Surreal.Mathematics;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Sprites
{
  public interface IPlannedSprite : IGrid<Color>
  {
    Point2 Offset { get; }
  }
}
