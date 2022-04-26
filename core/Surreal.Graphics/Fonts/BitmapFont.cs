using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.Graphics.Meshes;
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
public sealed class BitmapFont : IDisposable
{
  private readonly BitmapFontDescriptor descriptor;
  private readonly Image image;
  private readonly Texture texture;

  internal BitmapFont(IGraphicsServer server, BitmapFontDescriptor descriptor, Image image)
  {
    this.descriptor = descriptor;
    this.image      = image;

    texture = new Texture(server, TextureFormat.Rgba8888, TextureFilterMode.Point, TextureWrapMode.Clamp);
    texture.WritePixels<Color32>(image.Width, image.Height, image.Pixels);
  }

  public Image   Image   => image;
  public Texture Texture => texture;

  /// <summary>Measures the width of the given piece of text in the underlying font.</summary>
  public float MeasureWidth(string text)
  {
    return text.Length * (descriptor.GlyphWidth + descriptor.GlyphPadding);
  }

  public TextureRegion GetGlyph(int index)
  {
    Debug.Assert(index >= 0, "index >= 0");

    return new(texture)
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
    texture.Dispose();
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="BitmapFont"/>s.</summary>
public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
{
  private readonly IGraphicsServer server;

  public BitmapFontLoader(IGraphicsServer server)
  {
    this.server = server;
  }

  public override async ValueTask<BitmapFont> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var descriptor = await context.Path.DeserializeJsonAsync<BitmapFontDescriptor>(cancellationToken);
    var image = await context.Manager.LoadAsset<Image>(GetImagePath(context, descriptor), cancellationToken);

    return new BitmapFont(server, descriptor, image);
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
