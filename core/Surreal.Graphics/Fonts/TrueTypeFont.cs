using SixLabors.Fonts;
using Surreal.Assets;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Fonts;

/// <summary>Different weights for <see cref="TrueTypeFont"/> styles.</summary>
public enum FontWeight
{
  Normal,
  Italic,
  Bold
}

/// <summary>Utilities for working with <see cref="TrueTypeFont"/>s.</summary>
public static class TrueTypeFontExtensions
{
  public static async ValueTask<TrueTypeFont> LoadDefaultFontAsync(this IAssetManager manager)
  {
    return await manager.LoadBit536Async();
  }

  public static async ValueTask<TrueTypeFont> LoadBitBoyFontAsync(this IAssetManager manager)
  {
    return await manager.LoadAssetAsync<TrueTypeFont>("resx://Surreal.Graphics/Resources/fonts/bitboy8_v1.ttf");
  }

  public static async ValueTask<TrueTypeFont> LoadBit536Async(this IAssetManager manager)
  {
    return await manager.LoadAssetAsync<TrueTypeFont>("resx://Surreal.Graphics/Resources/fonts/bit536_v1.ttf");
  }

  /// <summary>Draws text on the given <see cref="SpriteBatch"/> with the given <see cref="BitmapFont"/>.</summary>
  public static void DrawText(
    this SpriteBatch batch,
    TrueTypeFont.RasterizedFont font,
    string text,
    Vector2 position,
    Color color,
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
      var glyph = font.GetGlyph(text[i]);

      if (text[i] == '\n')
      {
        position.Y -= glyph.Size.Y;
        position.X =  startPosition.X;
      }
      else
      {
        batch.Draw(glyph, position, glyph.Size, color);

        position.X += glyph.Size.X;
      }
    }
  }
}

/// <summary>A true type font that can be rendered at arbitrary sized.</summary>
public sealed class TrueTypeFont
{
  private readonly IGraphicsServer server;
  private FontFamily family;

  internal TrueTypeFont(IGraphicsServer server, FontFamily family)
  {
    this.server = server;
    this.family = family;
  }

  public RasterizedFont GetFont(float size, FontWeight weight = FontWeight.Normal)
  {
    var font = family.CreateFont(size, weight switch
    {
      FontWeight.Normal => FontStyle.Regular,
      FontWeight.Italic => FontStyle.Italic,
      FontWeight.Bold   => FontStyle.Bold,

      _ => throw new ArgumentOutOfRangeException(nameof(weight), weight, null)
    });

    return new RasterizedFont(server, font);
  }

  /// <summary>A <see cref="TrueTypeFont"/> that has been rasterized at a particular size.</summary>
  public sealed class RasterizedFont
  {
    private const string DefaultCharacterSet = "!@#$%^&*()_+abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ[]{},./ ";

    private readonly IGraphicsServer server;
    private readonly TextOptions options;

    internal RasterizedFont(IGraphicsServer server, Font font)
    {
      this.server = server;
      options     = new TextOptions(font);
    }

    /// <summary>Gets the relevant <see cref="TextureRegion"/> for rendering the given character.</summary>
    public TextureRegion GetGlyph(char character)
    {
      throw new NotImplementedException();
    }

    /// <summary>Measures the size of the given piece of text.</summary>
    public BoundingRect MeasureSize(ReadOnlySpan<char> text)
    {
      var size = TextMeasurer.Measure(text, options);

      return new BoundingRect(size.Left, size.Bottom, size.Right, size.Top);
    }

    /// <summary>Renders this font to a new <see cref="BitmapFont"/>.</summary>
    public BitmapFont ToBitmapFont(string characters = DefaultCharacterSet)
    {
      var atlas = new TextureAtlasBuilder();
      var renderer = new TextRenderer(new TextureGlyphRenderer(atlas));

      renderer.RenderText(characters, options);

      var texture = atlas.ToTexture(server, 3);
      var descriptor = new BitmapFontDescriptor
      {
        FilePath     = null,
        Columns      = 3,
        GlyphHeight  = 16,
        GlyphWidth   = 16,
        GlyphPadding = 0
      };

      return new BitmapFont(descriptor, texture, ownsTexture: true);
    }
  }

  /// <summary>A <see cref="IGlyphRenderer"/> that emits to texel data in a given <see cref="Texture"/>.</summary>
  private sealed class TextureGlyphRenderer : IGlyphRenderer
  {
    private TextureAtlasBuilder builder;
    private TextureAtlasBuilder.Cell currentCell;
    private Vector2 currentPoint;

    public TextureGlyphRenderer(TextureAtlasBuilder builder)
    {
      this.builder = builder;
    }

    void IGlyphRenderer.BeginText(FontRectangle bounds)
    {
    }

    bool IGlyphRenderer.BeginGlyph(FontRectangle bounds, GlyphRendererParameters paramaters)
    {
      currentCell = builder.AddCell(
        (int) MathF.Ceiling(bounds.Width),
        (int) MathF.Ceiling(bounds.Height)
      );

      return true;
    }

    void IGlyphRenderer.BeginFigure()
    {
    }

    void IGlyphRenderer.MoveTo(Vector2 point)
    {
      currentPoint = point;
    }

    void IGlyphRenderer.QuadraticBezierTo(Vector2 secondControlPoint, Vector2 point)
    {
      var curve = new QuadraticBezierCurve(currentPoint, secondControlPoint, point);

      currentCell.Span.DrawCurve(curve, Color32.White);
    }

    void IGlyphRenderer.CubicBezierTo(Vector2 secondControlPoint, Vector2 thirdControlPoint, Vector2 point)
    {
      var curve = new CubicBezierCurve(currentPoint, secondControlPoint, thirdControlPoint, point);

      currentCell.Span.DrawCurve(curve, Color32.White);
    }

    void IGlyphRenderer.LineTo(Vector2 point)
    {
      currentCell.Span.DrawLine(currentPoint, point, Color32.White);
    }

    void IGlyphRenderer.EndFigure()
    {
    }

    void IGlyphRenderer.EndGlyph()
    {
    }

    void IGlyphRenderer.EndText()
    {
    }
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="TrueTypeFont"/>s.</summary>
public sealed class TrueTypeFontLoader : AssetLoader<TrueTypeFont>
{
  private readonly IGraphicsServer server;

  public TrueTypeFontLoader(IGraphicsServer server)
  {
    this.server = server;
  }

  public override async ValueTask<TrueTypeFont> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();

    var collection = new FontCollection();
    collection.Add(stream);

    var family = collection.Families.Single();

    return new TrueTypeFont(server, family);
  }
}
