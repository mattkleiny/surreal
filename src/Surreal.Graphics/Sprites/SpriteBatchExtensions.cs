using System;
using System.Numerics;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics.Sprites
{
  public static class SpriteBatchExtensions
  {
    // TODO: fix the pivoting mechanics

    public static void DrawPivoted(
      this SpriteBatch batch,
      TextureRegion region,
      Vector2 position,
      Pivot pivot = default,
      float rotation = 0f,
      float scale = 1f)
    {
      var scaledHalfWidth  = region.Texture.Width * scale * pivot.X;
      var scaledHalfHeight = region.Texture.Height * scale * pivot.Y;

      batch.Draw(
        region: region.Texture,
        x: position.X - scaledHalfWidth,
        y: position.Y - scaledHalfHeight,
        rotation: rotation,
        width: region.Texture.Width * scale,
        height: region.Texture.Height * scale
      );
    }

    public static void DrawText(
      this SpriteBatch batch,
      ReadOnlySpan<char> text,
      BitmapFont font,
      Vector2 position,
      Color color,
      float scale = 1f)
    {
      batch.Color = color;

      for (var i = 0; i < text.Length; i++)
      {
        var symbol = text[i];

        if (font.TryGetGlyph(symbol, out var glyph))
        {
          batch.Draw(
            region: glyph.Region,
            x: position.X + glyph.Bearing.X * scale,
            y: position.Y - (glyph.Size.Y - glyph.Bearing.Y) * scale,
            rotation: 0f,
            width: glyph.Size.X * scale,
            height: glyph.Size.Y * scale
          );

          position.X += glyph.Size.X * scale;
        }
      }
    }
  }
}