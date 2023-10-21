using Surreal.Colors;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Maths;
using Surreal.Resources;

namespace Surreal.Graphics.Fonts;

/// <summary>
/// Horizontal alignments for text rendering.
/// </summary>
public enum HorizontalAlignment
{
  Left,
  Center
}

/// <summary>
/// Vertical alignments for text rendering.
/// </summary>
public enum VerticalAlignment
{
  Top,
  Center
}

/// <summary>
/// Utilities for working with <see cref="BitmapFont" />s.
/// </summary>
public static class BitmapFontExtensions
{
  public static async Task<BitmapFont> LoadDefaultBitmapFontAsync(this IResourceManager manager)
  {
    return await manager.LoadResourceAsync<BitmapFont>("resx://Surreal.Graphics/Resources/fonts/IBM.font");
  }

  /// <summary>
  /// Draws text on the given <see cref="SpriteBatch" /> with the given <see cref="BitmapFont" />.
  /// </summary>
  public static void DrawText(this SpriteBatch batch, BitmapFont font, string text, Vector2 position)
  {
    DrawText(batch, font, text, position, Color.White);
  }

  /// <summary>
  /// Draws text on the given <see cref="SpriteBatch" /> with the given <see cref="BitmapFont" />.
  /// </summary>
  public static void DrawText(this SpriteBatch batch, BitmapFont font, string text, Vector2 position, Color color)
  {
    DrawText(batch, font, text, position, Vector2.One, color);
  }

  /// <summary>
  /// Draws text on the given <see cref="SpriteBatch" /> with the given <see cref="BitmapFont" />.
  /// </summary>
  public static void DrawText(
    this SpriteBatch batch,
    BitmapFont font,
    string text,
    Vector2 position,
    Vector2 scale,
    Color color,
    float angle = 0f,
    HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
    VerticalAlignment verticalAlignment = VerticalAlignment.Top
  )
  {
    // TODO: implement galley system for layout out text?
    var size = font.MeasureSize(text);

    if (horizontalAlignment == HorizontalAlignment.Center)
    {
      position.X -= size.Width / 2f * scale.X;
    }

    if (verticalAlignment == VerticalAlignment.Center)
    {
      position.Y += size.Height / 2f * scale.Y;
    }

    var startPosition = position;

    for (var i = 0; i < text.Length; i++)
    {
      var character = text[i];
      var glyph = font.GetGlyph(character);

      if (character == '\n')
      {
        position.Y -= glyph.Size.Y * scale.Y;
        position.X = startPosition.X;
      }
      else
      {
        var targetScale = new Vector2(
          glyph.Size.X * scale.X,
          glyph.Size.Y * scale.Y
        );

        batch.Draw(glyph, position, targetScale, angle, color);

        position.X += glyph.Size.X * scale.X;
      }
    }
  }
}

/// <summary>
/// Describes the structure of a <see cref="BitmapFont" />.
/// </summary>
internal sealed record BitmapFontDescriptor
{
  public string? FilePath { get; set; }
  public int GlyphWidth { get; set; }
  public int GlyphHeight { get; set; }
  public int GlyphPadding { get; set; }
  public int Columns { get; set; }
}

/// <summary>
/// A font represented as small bitmaps.
/// </summary>
public sealed class BitmapFont : IDisposable
{
  private readonly BitmapFontDescriptor _descriptor;
  private readonly bool _ownsTexture;

  internal BitmapFont(BitmapFontDescriptor descriptor, Texture texture, bool ownsTexture = false)
  {
    _descriptor = descriptor;
    _ownsTexture = ownsTexture;

    Texture = texture;
  }

  public Texture Texture { get; }

  public void Dispose()
  {
    if (_ownsTexture)
    {
      Texture.Dispose();
    }
  }

  /// <summary>
  /// Measures the width of the given piece of text in the underlying font.
  /// </summary>
  public Rectangle MeasureSize(string text)
  {
    var lineCount = 0;
    var longestLine = 0;
    var currentLine = 0;

    for (var i = 0; i < text.Length; i++)
    {
      currentLine += 1;

      if (currentLine > longestLine)
      {
        longestLine = currentLine;
      }

      if (text[i] == '\n')
      {
        lineCount++;
        currentLine = 0;
      }
    }

    return new Rectangle(
      0,
      0,
      longestLine * (_descriptor.GlyphWidth + _descriptor.GlyphPadding),
      lineCount * (_descriptor.GlyphHeight + _descriptor.GlyphPadding)
    );
  }

  public TextureRegion GetGlyph(int index)
  {
    Debug.Assert(index >= 0, "index >= 0");

    return new TextureRegion(Texture)
    {
      Offset = new Point2(
        index % _descriptor.Columns * (_descriptor.GlyphWidth + _descriptor.GlyphPadding),
        index / _descriptor.Columns * (_descriptor.GlyphHeight + _descriptor.GlyphPadding)
      ),
      Size = new Point2(
        _descriptor.GlyphWidth,
        _descriptor.GlyphHeight
      )
    };
  }
}
