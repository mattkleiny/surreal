using System.Globalization;
using SixLabors.Fonts;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Fonts;

/// <summary>A font represented as mathematical curves.</summary>
public sealed class TrueTypeFont
{
  private readonly GlyphRenderer renderer = new();
  private readonly FontFamily fontFamily;

  internal TrueTypeFont(FontFamily fontFamily)
  {
    this.fontFamily = fontFamily;
  }

  public void DrawText(ReadOnlySpan<char> message)
  {
    throw new NotImplementedException();
  }

  /// <summary>A <see cref="IColorGlyphRenderer"/> for true type font rasterisation.</summary>
  private sealed class GlyphRenderer : IColorGlyphRenderer
  {
    public void BeginFigure()
    {
      throw new NotImplementedException();
    }

    public void MoveTo(Vector2 point)
    {
      throw new NotImplementedException();
    }

    public void QuadraticBezierTo(Vector2 secondControlPoint, Vector2 point)
    {
      throw new NotImplementedException();
    }

    public void CubicBezierTo(Vector2 secondControlPoint, Vector2 thirdControlPoint, Vector2 point)
    {
      throw new NotImplementedException();
    }

    public void LineTo(Vector2 point)
    {
      throw new NotImplementedException();
    }

    public void EndFigure()
    {
      throw new NotImplementedException();
    }

    public void EndGlyph()
    {
      throw new NotImplementedException();
    }

    public bool BeginGlyph(FontRectangle bounds, GlyphRendererParameters paramaters)
    {
      throw new NotImplementedException();
    }

    public void EndText()
    {
      throw new NotImplementedException();
    }

    public void BeginText(FontRectangle bounds)
    {
      throw new NotImplementedException();
    }

    public void SetColor(GlyphColor color)
    {
      throw new NotImplementedException();
    }
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="TrueTypeFont"/>s.</summary>
public sealed class TrueTypeFontLoader : AssetLoader<TrueTypeFont>
{
  private readonly CultureInfo culture;
  private readonly FontCollection fontCollection = new();

  public TrueTypeFontLoader()
    : this(CultureInfo.InvariantCulture)
  {
  }

  public TrueTypeFontLoader(CultureInfo culture)
  {
    this.culture = culture;
  }

  public IReadOnlyFontCollection FontCollection => fontCollection;

  public override async ValueTask<TrueTypeFont> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();

    var fontFamily = fontCollection.Install(stream, culture);

    return new TrueTypeFont(fontFamily);
  }
}
