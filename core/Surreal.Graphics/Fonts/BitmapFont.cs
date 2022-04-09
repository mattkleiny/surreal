using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Fonts;

/// <summary>A font represented as small bitmaps.</summary>
public sealed class BitmapFont : IDisposable
{
  private readonly BitmapFontDescriptor descriptor;
  private readonly Image image;
  private readonly Texture texture;

  internal BitmapFont(IGraphicsServer server, BitmapFontDescriptor descriptor, Image image)
  {
    this.descriptor = descriptor;
    this.image = image;

    texture = new Texture(server, TextureFormat.Rgba8888, TextureFilterMode.Point, TextureWrapMode.Clamp);
    texture.WritePixels<Color32>(image.Width, image.Height, image.Pixels);
  }

  public Image   Image   => image;
  public Texture Texture => texture;

  public TextureRegion GetGlyph(int index)
  {
    Debug.Assert(index >= 0, "index >= 0");

    return new(texture)
    {
      Offset = new Point2(
        X: index % descriptor.Columns,
        Y: index / descriptor.Columns
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

/// <summary>Utilities for working with <see cref="BitmapFont"/>s in a <see cref="SpriteBatch"/>.</summary>
public static class SpriteBatchExtensions
{
  public static void DrawText(this SpriteBatch batch, BitmapFont font, string text, Vector2 position)
  {
    for (var i = 0; i < text.Length; i++)
    {
      var glyph = font.GetGlyph(text[i]);

      batch.Draw(glyph, position, glyph.Size);

      position.X += glyph.Size.X;
    }
  }
}

/// <summary>Describes the structure of a <see cref="BitmapFont"/>.</summary>
internal sealed record BitmapFontDescriptor
{
  public string? FilePath    { get; set; }
  public int     GlyphWidth  { get; set; }
  public int     GlyphHeight { get; set; }
  public int     Columns     { get; set; }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="BitmapFont"/>s.</summary>
public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
{
  private readonly IGraphicsServer server;

  public BitmapFontLoader(IGraphicsServer server)
  {
    this.server = server;
  }

  public override async ValueTask<BitmapFont> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    var descriptor = await context.Path.DeserializeJsonAsync<BitmapFontDescriptor>(progressToken.CancellationToken);
    var image = await context.Manager.LoadAssetAsync<Image>(GetImagePath(context, descriptor), progressToken);

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

      return context.Path.GetDirectoryName().Resolve(descriptor.FilePath);
    }

    return descriptor.FilePath ?? context.Path.ChangeExtension("png");
  }
}
