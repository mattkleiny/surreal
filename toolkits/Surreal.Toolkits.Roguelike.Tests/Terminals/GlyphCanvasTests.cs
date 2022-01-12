using Surreal.Mathematics;

namespace Surreal.Terminals;

public class GlyphCanvasTests
{
  [Test, AutoFixture]
  public void it_should_remember_dirty_glyphs(GlyphCanvas.RenderGlyphCallback callback)
  {
    var canvas = new GlyphCanvas(15, 9);

    canvas.SetGlyph(14, 8, new Glyph('x', Color.White, Color.Black));

    canvas.Render(callback);
    canvas.Render(callback);
    canvas.Render(callback);

    callback.Received(1).Invoke(14, 8, Arg.Any<Glyph>());
  }
}
