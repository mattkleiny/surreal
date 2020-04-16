using System.Numerics;
using Isaac.Core.Maps;
using Surreal.Collections;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;

namespace Isaac.Graphics
{
  internal static class RenderingExtensions
  {
    public static void Render(
      this Floor floor,
      SpriteBatch batch,
      Atlas<TextureRegion> textures,
      float scale = 1f,
      Vector2 offset = default)
    {
      for (var y = 0; y < floor.Height; y++)
      for (var x = 0; x < floor.Width; x++)
      {
        floor[x, y]?.Render(batch, textures, scale, offset);
      }
    }

    public static void Render(
      this Room room,
      SpriteBatch batch,
      Atlas<TextureRegion> textures,
      float scale = 1f,
      Vector2 offset = default)
    {
      // TODO: batch by texture type/tile properties/etc
      // TODO: move this to a dedicated tile map renderer

      for (var y = 0; y < room.Height; y++)
      for (var x = 0; x < room.Width; x++)
      {
        var tile = room[x, y];
        if (tile.Texture == null) continue;

        var texture = textures[tile.Texture];

        batch.Draw(
          texture,
          x * scale * texture.Width + offset.X,
          y * scale * texture.Height + offset.Y,
          width: texture.Width * scale,
          height: texture.Height * scale,
          rotation: 0f
        );
      }
    }
  }
}
