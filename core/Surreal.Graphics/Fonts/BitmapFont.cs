using Surreal.Assets;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Fonts;

/// <summary>Alignments for text rendering.</summary>
public enum TextAlignment
{
  Left,
  Center,
}

/// <summary>Utilities for working with <see cref="BitmapFont"/>s in a <see cref="SpriteBatch"/>.</summary>
public static class BitmapFontExtensions
{
  /// <summary>Loads the default system font from Surreal.</summary>
  public static async ValueTask<BitmapFont> LoadDefaultFontAsync(this IAssetManager manager)
  {
    return await manager.LoadAsset<BitmapFont>("resx://Surreal.Graphics/Resources/fonts/IBM.font");
  }

  /// <summary>Draws text on the given <see cref="SpriteBatch"/> with the given font.</summary>
  public static void DrawText(this SpriteBatch batch, BitmapFont font, string text, Vector2 position, Color color, TextAlignment alignment = TextAlignment.Left)
  {
    if (alignment == TextAlignment.Center)
    {
      position.X -= font.MeasureWidth(text) / 2f;
    }

    for (var i = 0; i < text.Length; i++)
    {
      var glyph = font.GetGlyph(text[i]);

      batch.Draw(glyph, position, glyph.Size, color);

      position.X += glyph.Size.X;
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
public sealed class BitmapFont
{
  private readonly BitmapFontDescriptor descriptor;

  internal BitmapFont(BitmapFontDescriptor descriptor, Texture texture)
  {
    this.descriptor = descriptor;

    Texture = texture;
  }

  public Texture Texture { get; }

  /// <summary>Measures the width of the given piece of text in the underlying font.</summary>
  public float MeasureWidth(string text)
  {
    return text.Length * (descriptor.GlyphWidth + descriptor.GlyphPadding);
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
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="BitmapFont"/>s.</summary>
public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
{
  public override async ValueTask<BitmapFont> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var descriptor = await context.Path.DeserializeJsonAsync<BitmapFontDescriptor>(cancellationToken);
    var texture = await context.LoadDependencyAsync<Texture>(GetImagePath(context, descriptor), cancellationToken);

    return new BitmapFont(descriptor, texture);
  }

  private static VirtualPath GetImagePath(AssetLoaderContext context, BitmapFontDescriptor descriptor)
  {
    if (descriptor.FilePath != null)
    {
      if (Path.IsPathRooted(descriptor.FilePath))
      {
        return descriptor.FilePath;
      }

      return context.Path.GetDirectory().Resolve(descriptor.FilePath);
    }

    return descriptor.FilePath ?? context.Path.ChangeExtension("png");
  }
}
