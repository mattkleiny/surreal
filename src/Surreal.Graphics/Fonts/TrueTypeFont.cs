using System.Globalization;
using SixLabors.Fonts;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Fonts;

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

public sealed class TrueTypeFontLoader : AssetLoader<TrueTypeFont>
{
  public override async Task<TrueTypeFont> LoadAsync(VirtualPath path, IAssetContext context)
  {
    await using var stream = await path.OpenInputStreamAsync();

    var fonts      = new FontCollection();
    var fontFamily = fonts.Install(stream, CultureInfo.InvariantCulture);

    return new TrueTypeFont(fontFamily);
  }
}
