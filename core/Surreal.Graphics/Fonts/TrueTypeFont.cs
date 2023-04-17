using System.Globalization;
using SixLabors.Fonts;
using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Fonts;

/// <summary>
/// Different weights for <see cref="TrueTypeFont" /> styles.
/// </summary>
public enum FontWeight
{
  Normal,
  Italic,
  Bold
}

/// <summary>
/// Utilities for working with <see cref="TrueTypeFont" />s.
/// </summary>
public static class TrueTypeFontExtensions
{
  public static async Task<TrueTypeFont> LoadDefaultFontAsync(this IAssetManager manager)
  {
    return await manager.LoadBit536Async();
  }

  public static async Task<TrueTypeFont> LoadBitBoyFontAsync(this IAssetManager manager)
  {
    return await manager.LoadAssetAsync<TrueTypeFont>("resx://Surreal.Graphics/Resources/fonts/bitboy8_v1.ttf");
  }

  public static async Task<TrueTypeFont> LoadBit536Async(this IAssetManager manager)
  {
    return await manager.LoadAssetAsync<TrueTypeFont>("resx://Surreal.Graphics/Resources/fonts/bit536_v1.ttf");
  }
}

/// <summary>
/// A true type font that can be rendered at arbitrary sized.
/// </summary>
public sealed class TrueTypeFont
{
  private readonly IGraphicsServer _server;
  private FontFamily _family;

  internal TrueTypeFont(IGraphicsServer server, FontFamily family)
  {
    _server = server;
    _family = family;
  }

  public RasterFont GetFont(float size, FontWeight weight = FontWeight.Normal)
  {
    var font = _family.CreateFont(size, weight switch
    {
      FontWeight.Normal => FontStyle.Regular,
      FontWeight.Italic => FontStyle.Italic,
      FontWeight.Bold => FontStyle.Bold,

      _ => throw new ArgumentOutOfRangeException(nameof(weight), weight, null)
    });

    return new RasterFont(_server, font);
  }

  /// <summary>
  /// A <see cref="TrueTypeFont" /> that can be rasterized at a particular size.
  /// </summary>
  public sealed class RasterFont
  {
    private static readonly string AsciiCharacterSet;
    private readonly TextOptions _options;

    private readonly IGraphicsServer _server;

    static RasterFont()
    {
      // build default ASCII character set
      var builder = new StringBuilder();

      for (var i = 0; i < 256; i++) builder.Append((char)i);

      AsciiCharacterSet = builder.ToString();
    }

    internal RasterFont(IGraphicsServer server, Font font)
    {
      _server = server;
      _options = new TextOptions(font);
    }

    /// <summary>
    /// Measures the size of the given piece of text.
    /// </summary>
    public Rectangle MeasureSize(ReadOnlySpan<char> text)
    {
      var size = TextMeasurer.Measure(text, _options);

      return new Rectangle(size.Left, size.Bottom, size.Right, size.Top);
    }

    /// <summary>
    /// Renders this font to a new <see cref="BitmapFont" />.
    /// </summary>
    public BitmapFont ToBitmapFont()
    {
      return ToBitmapFont(AsciiCharacterSet);
    }

    /// <summary>
    /// Renders this font to a new <see cref="BitmapFont" />.
    /// </summary>
    public BitmapFont ToBitmapFont(string characters)
    {
      var atlas = new TextureAtlasGlyphRenderer();
      var renderer = new TextRenderer(atlas);

      renderer.RenderText(characters, _options);

      var texture = atlas.ToTexture(_server, 3);
      var descriptor = new BitmapFontDescriptor
      {
        FilePath = null,
        Columns = 3,
        GlyphHeight = 16,
        GlyphWidth = 16,
        GlyphPadding = 0
      };

      return new BitmapFont(descriptor, texture, true);
    }
  }

  /// <summary>
  /// A <see cref="IGlyphRenderer" /> that emits to texel data in a given <see cref="Texture" />.
  /// </summary>
  private sealed class TextureAtlasGlyphRenderer : TextureAtlasBuilder, IGlyphRenderer
  {
    private Cell _currentCell;
    private Vector2 _currentPoint;

    void IGlyphRenderer.BeginText(FontRectangle bounds)
    {
    }

    bool IGlyphRenderer.BeginGlyph(FontRectangle bounds, GlyphRendererParameters paramaters)
    {
      _currentCell = AddCell(
        (int)MathF.Ceiling(bounds.Width),
        (int)MathF.Ceiling(bounds.Height)
      );

      return true;
    }

    void IGlyphRenderer.BeginFigure()
    {
    }

    void IGlyphRenderer.MoveTo(Vector2 point)
    {
      _currentPoint = point;
    }

    void IGlyphRenderer.QuadraticBezierTo(Vector2 secondControlPoint, Vector2 point)
    {
      var curve = new QuadraticBezierCurve(_currentPoint, secondControlPoint, point);

      _currentCell.Span.DrawCurve(curve, Color32.White);
    }

    void IGlyphRenderer.CubicBezierTo(Vector2 secondControlPoint, Vector2 thirdControlPoint, Vector2 point)
    {
      var curve = new CubicBezierCurve(_currentPoint, secondControlPoint, thirdControlPoint, point);

      _currentCell.Span.DrawCurve(curve, Color32.White);
    }

    void IGlyphRenderer.LineTo(Vector2 point)
    {
      _currentCell.Span.DrawLine(_currentPoint, point, Color32.White);
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

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="TrueTypeFont" />s.
/// </summary>
public sealed class TrueTypeFontLoader : AssetLoader<TrueTypeFont>
{
  private readonly IGraphicsServer _server;

  public TrueTypeFontLoader(IGraphicsServer server)
  {
    _server = server;
  }

  public override async Task<TrueTypeFont> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();

    // configure a font collection, with a single font
    var collection = new FontCollection();
    collection.Add(stream, CultureInfo.InvariantCulture);

    // we're expecting a single font family per file
    var family = collection.Families.Single();

    return new TrueTypeFont(_server, family);
  }
}
