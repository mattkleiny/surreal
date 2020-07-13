using System;
using System.Numerics;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.Primitives;
using Surreal.Assets;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics.Curves;
using Surreal.Mathematics.Grids;

namespace Surreal.Graphics.Fonts {
  public sealed class TrueTypeFont {
    private readonly FontCollection collection;

    public TrueTypeFont(FontCollection collection) {
      this.collection = collection;
    }

    public Pixmap ToPixmap(ReadOnlySpan<char> text, string name, float size, FontStyle style = FontStyle.Regular) {
      var family = collection.Find(name);
      var font   = family.CreateFont(size, style);

      var rasterizer = new GlyphRasterizer(256, 256, 16, 16, Color.White);
      var renderer   = new TextRenderer(rasterizer);
      var options    = new RendererOptions(font);

      for (var i = 0; i < text.Length; i++) {
        // HACK: feed one character in at a time, to prevent automatic line stepping/wrapping
        renderer.RenderText(text[i..(i + 1)], options);
      }

      return rasterizer.Pixmap;
    }

    private sealed class GlyphRasterizer : IGlyphRenderer {
      private readonly SpriteSheetCutter cutter;
      private readonly Color             color;

      private IPlannedSprite? glyph;
      private PointF          startPoint;

      public GlyphRasterizer(int width, int height, int widthPerCell, int heightPerCell, Color color) {
        this.color = color;

        cutter = new SpriteSheetCutter(width, height, widthPerCell, heightPerCell);
      }

      public Pixmap Pixmap => cutter.Pixmap;

      public void MoveTo(PointF point) {
        startPoint = point;
      }

      public void LineTo(PointF endPoint) {
        glyph!.DrawLine(
            from: new Vector2(startPoint.X, startPoint.Y),
            to: new Vector2(endPoint.X, endPoint.Y),
            value: color
        );

        startPoint = endPoint;
      }

      public void QuadraticBezierTo(PointF controlPoint, PointF endPoint) {
        glyph!.DrawCurve(
            new QuadraticBezierCurve(
                startPoint: new Vector2(startPoint.X, startPoint.Y),
                controlPoint: new Vector2(controlPoint.X, controlPoint.Y),
                endPoint: new Vector2(endPoint.X, endPoint.Y)
            ),
            value: color,
            resolution: 16
        );
      }

      public void CubicBezierTo(PointF controlPoint1, PointF controlPoint2, PointF endPoint) {
        glyph!.DrawCurve(
            new CubicBezierCurve(
                startPoint: new Vector2(startPoint.X, startPoint.Y),
                controlPoint1: new Vector2(controlPoint1.X, controlPoint1.Y),
                controlPoint2: new Vector2(controlPoint2.X, controlPoint2.Y),
                endPoint: new Vector2(endPoint.X, endPoint.Y)
            ),
            value: color,
            resolution: 16
        );
      }

      public bool BeginGlyph(RectangleF bounds, GlyphRendererParameters paramaters) {
        glyph = cutter.AddSprite();

        return true;
      }

      public void EndGlyph() {
        glyph = null;
      }

      public void BeginText(RectangleF bounds) {
      }

      public void EndText() {
      }

      public void BeginFigure() {
      }

      public void EndFigure() {
      }
    }

    public sealed class Loader : AssetLoader<TrueTypeFont> {
      public override async Task<TrueTypeFont> LoadAsync(Path path, IAssetLoaderContext context) {
        await using var stream = await path.OpenInputStreamAsync();

        // TODO: clean this up?
        var collection = new FontCollection();
        collection.Install(stream);

        return new TrueTypeFont(collection);
      }
    }
  }
}