using Surreal.Graphics.Textures;
using Surreal.Mathematics.Grids;

namespace Surreal.Graphics.Sprites
{
  public interface IPlannedSprite : IGrid<Color>
  {
    int OffsetX { get; }
    int OffsetY { get; }

    PixmapRegion Region { get; }
  }
}
