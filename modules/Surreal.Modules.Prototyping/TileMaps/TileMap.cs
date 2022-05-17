using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.TileMaps;

/// <summary>A fixed-size tile map that can be rendered to a <see cref="SpriteBatch"/>.</summary>
public sealed class TileMap<T>
{
  public TileMap(int width, int height)
  {
    Tiles = new Grid<T>(width, height);
  }

  public int Width  => Tiles.Width;
  public int Height => Tiles.Height;

  /// <summary>The actual tile content of the <see cref="TileMap{T}"/>.</summary>
  public Grid<T> Tiles { get; }

  /// <summary>Draws the tilemap to the given <see cref="SpriteBatch"/> with the given sprite selector.</summary>
  public void Draw(SpriteBatch batch, Vector2 offset, Vector2 tileSize, Vector2 mousePos, Func<T, Rectangle, (TextureRegion, Color)> spriteSelector)
  {
    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var tile = Tiles[x, y];
      if (tile != null)
      {
        // TODO: sprite pivots?
        var position = offset + new Vector2(x * tileSize.X, y * tileSize.Y) + tileSize / 2f;
        var (sprite, color) = spriteSelector(tile, Rectangle.Create(position, tileSize));

        batch.Draw(sprite, position, sprite.Size, color);
      }
    }
  }
}
