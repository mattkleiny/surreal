using Surreal.Assets;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Fonts;

/// <summary>Horizontal alignments for text rendering.</summary>
public enum HorizontalAlignment
{
  Left,
  Center
}

/// <summary>Vertical alignments for text rendering.</summary>
public enum VerticalAlignment
{
  Top,
  Center
}

/// <summary>Utilities for working with <see cref="BitmapFont"/>s.</summary>
public static class BitmapFontExtensions
{
  public static async Task<BitmapFont> LoadDefaultBitmapFontAsync(this IAssetManager manager)
  {
    return await manager.LoadAssetAsync<BitmapFont>("resx://Surreal.Graphics/Resources/fonts/IBM.font");
  }

  /// <summary>Draws text on the given <see cref="SpriteBatch"/> with the given <see cref="BitmapFont"/>.</summary>
  public static void DrawText(this SpriteBatch batch, BitmapFont font, string text, Vector2 position)
    => DrawText(batch, font, text, position, Color.White);

  /// <summary>Draws text on the given <see cref="SpriteBatch"/> with the given <see cref="BitmapFont"/>.</summary>
  public static void DrawText(this SpriteBatch batch, BitmapFont font, string text, Vector2 position, Color color)
    => DrawText(batch, font, text, position, Vector2.One, color);

  /// <summary>Draws text on the given <see cref="SpriteBatch"/> with the given <see cref="BitmapFont"/>.</summary>
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
    var size = font.MeasureSize(text);

    if (horizontalAlignment == HorizontalAlignment.Center)
    {
      position.X -= size.Width / 2f;
    }

    if (verticalAlignment == VerticalAlignment.Center)
    {
      position.Y += size.Height / 2f;
    }

    var startPosition = position;

    for (var i = 0; i < text.Length; i++)
    {
      var character = text[i];
      var glyph = font.GetGlyph(character);

      if (character == '\n')
      {
        position.Y -= glyph.Size.Y * scale.Y;
        position.X =  startPosition.X;
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

/// <summary>Describes the structure of a <see cref="BitmapFont"/>.</summary>
internal sealed record BitmapFontDescriptor
{
  public string? FilePath     { get; set; }
  public int     GlyphWidth   { get; set; }
  public int     GlyphHeight  { get; set; }
  public int     GlyphPadding { get; set; }
  public int     Columns      { get; set; }
}

/// <summary>A font represented as small bitmaps.</summary>
public sealed class BitmapFont : IDisposable
{
  private readonly BitmapFontDescriptor descriptor;
  private readonly bool ownsTexture;

  internal BitmapFont(BitmapFontDescriptor descriptor, Texture texture, bool ownsTexture = false)
  {
    this.descriptor  = descriptor;
    this.ownsTexture = ownsTexture;

    Texture = texture;
  }

  public Texture Texture { get; }

  /// <summary>Measures the width of the given piece of text in the underlying font.</summary>
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
      Left: 0,
      Top: 0,
      Right: longestLine * (descriptor.GlyphWidth + descriptor.GlyphPadding),
      Bottom: lineCount * (descriptor.GlyphHeight + descriptor.GlyphPadding)
    );
  }

  public TextureRegion GetGlyph(int index)
  {
    Debug.Assert(index >= 0, "index >= 0");

    return new TextureRegion(Texture)
    {
      Offset = new Point2(
        X: (index % descriptor.Columns) * (descriptor.GlyphWidth + descriptor.GlyphPadding),
        Y: (index / descriptor.Columns) * (descriptor.GlyphHeight + descriptor.GlyphPadding)
      ),
      Size = new Point2(
        X: descriptor.GlyphWidth,
        Y: descriptor.GlyphHeight
      )
    };
  }

  public void Dispose()
  {
    if (ownsTexture)
    {
      Texture.Dispose();
    }
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="BitmapFont"/>s.</summary>
public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
{
  public override async Task<BitmapFont> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var descriptor = await context.Path.DeserializeJsonAsync<BitmapFontDescriptor>(cancellationToken);

    var imagePath = GetImagePath(context.Path, descriptor);
    var texture = await context.LoadAsync<Texture>(imagePath, cancellationToken);

    return new BitmapFont(descriptor, texture, ownsTexture: false);
  }

  private static VirtualPath GetImagePath(VirtualPath descriptorPath, BitmapFontDescriptor descriptor)
  {
    if (descriptor.FilePath != null)
    {
      if (Path.IsPathRooted(descriptor.FilePath))
      {
        return descriptor.FilePath;
      }

      return descriptorPath.GetDirectory().Resolve(descriptor.FilePath);
    }

    return descriptorPath.ChangeExtension("png");
  }
}
