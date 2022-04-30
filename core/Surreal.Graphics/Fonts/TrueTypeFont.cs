using SixLabors.Fonts;
using Surreal.Assets;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;

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
public sealed class TrueTypeFont : IDisposable
{
  private readonly IGraphicsServer server;
  private readonly Dictionary<(float Size, FontWeight Weight), RasterizedFont> fontCache = new();

  private FontFamily family;

  internal TrueTypeFont(IGraphicsServer server, FontFamily family)
  {
    this.server = server;
    this.family = family;
  }

  public RasterizedFont GetFont(float size, FontWeight weight = FontWeight.Normal)
  {
    if (!fontCache.TryGetValue((size, weight), out var result))
    {
      var font = family.CreateFont(size, weight switch
      {
        FontWeight.Normal => FontStyle.Regular,
        FontWeight.Italic => FontStyle.Italic,
        FontWeight.Bold   => FontStyle.Bold,

        _ => throw new ArgumentOutOfRangeException(nameof(weight), weight, null)
      });

      fontCache[(size, weight)] = result = new RasterizedFont(server, font);
    }

    return result;
  }

  public void Dispose()
  {
    foreach (var font in fontCache.Values)
    {
      font.Dispose();
    }

    fontCache.Clear();
  }

  /// <summary>A <see cref="TrueTypeFont"/> that has been rasterized at a particular size.</summary>
  public sealed class RasterizedFont : IDisposable
  {
    // TODO: create a glyph cache, which allows writing new texture data for each requested character in a string
    private readonly Font font;
    private readonly TextOptions options;
    private readonly Texture texture;
    private readonly TextRenderer renderer;

    internal RasterizedFont(IGraphicsServer server, Font font)
    {
      this.font = font;

      texture  = new Texture(server, TextureFormat.Rgba8888);
      options  = new TextOptions(font);
      renderer = new TextRenderer(new TextureGlyphRenderer());
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

    public void Dispose()
    {
      texture.Dispose();
    }
  }

  /// <summary>A <see cref="IGlyphRenderer"/> that emits to texel data in a given <see cref="Texture"/>.</summary>
  private sealed class TextureGlyphRenderer : IGlyphRenderer
  {
    void IGlyphRenderer.BeginText(FontRectangle bounds)
    {
      // called before any thing else to provide access to the total required size to render the text
    }

    bool IGlyphRenderer.BeginGlyph(FontRectangle bounds, GlyphRendererParameters paramaters)
    {
      // You can return false to skip all the figures within the glyph (if you return false EndGlyph will still be called)

      return false;
    }

    void IGlyphRenderer.BeginFigure()
    {
      // called at the start of the figure within the single glyph/layer
      // glyphs are rendered as a serise of arcs, lines and movements
      // which together describe a complex shape.
    }

    void IGlyphRenderer.MoveTo(Vector2 point)
    {
      // move current point to location marked by point without describing a line;
    }

    void IGlyphRenderer.QuadraticBezierTo(Vector2 secondControlPoint, Vector2 point)
    {
      // describes Quadratic Bezier curve from the 'current point' using the
      // 'second control point' and final 'point' leaving the 'current point'
      // at 'point'
    }

    void IGlyphRenderer.CubicBezierTo(Vector2 secondControlPoint, Vector2 thirdControlPoint, Vector2 point)
    {
      // describes Cubic Bezier curve from the 'current point' using the
      // 'second control point', 'third control point' and final 'point'
      // leaving the 'current point' at 'point'
    }

    void IGlyphRenderer.LineTo(Vector2 point)
    {
      // describes straight line from the 'current point' to the final 'point'
      // leaving the 'current point' at 'point'
    }

    void IGlyphRenderer.EndFigure()
    {
      // Called after the figure has completed denoting a straight line should
      // be drawn from the current point to the first point
    }

    void IGlyphRenderer.EndGlyph()
    {
      // says the all figures have completed for the current glyph/layer.
      // NOTE this will be called even if BeginGlyph return false.
    }

    void IGlyphRenderer.EndText()
    {
      //once all glyphs/layers have been drawn this is called.
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
