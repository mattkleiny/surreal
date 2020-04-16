using Surreal.Collections;
using Surreal.Graphics.Raycasting;
using Surreal.Graphics.Textures;

namespace Prelude.Core
{
  public sealed class TileMapRenderer : SoftwareRaycastRenderer<Tile>
  {
    private const int Width  = 640 / 2;
    private const int Height = Width / 16 * 9;

    public TileMapRenderer(RaycastCamera camera, Atlas<PixmapRegion> textures)
      : base(camera, (Width, Height), textures)
    {
    }
  }
}
