using System.Globalization;
using SixLabors.Fonts;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Fonts;

/// <summary>A font represented as mathematical curves.</summary>
public sealed class TrueTypeFont
{
  private readonly FontFamily    font;
  private readonly GlyphRenderer renderer = new();

  internal TrueTypeFont(FontFamily font)
  {
    this.font = font;
  }

  public void DrawText(ReadOnlySpan<char> message)
  {
    throw new NotImplementedException();
  }

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

  public TrueTypeFontLoader()
    : this(CultureInfo.InvariantCulture)
  {
  }

  public TrueTypeFontLoader(CultureInfo culture)
  {
    this.culture = culture;
  }

  public override async Task<TrueTypeFont> LoadAsync(VirtualPath path, IAssetContext context, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    var fonts      = new FontCollection();
    var fontFamily = fonts.Install(stream, culture);

    return new TrueTypeFont(fontFamily);
  }
}
