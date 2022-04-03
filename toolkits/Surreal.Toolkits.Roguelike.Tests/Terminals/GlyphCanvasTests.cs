using Surreal.Mathematics;

namespace Surreal.Terminals;

public class GlyphCanvasTests
{
  [Test, AutoFixture]
  public void it_should_remember_dirty_glyphs(IGlyphRenderer renderer)
  {
    var canvas = new GlyphCanvas(15, 9, renderer)
    {
      [14, 8] = new('x', Color.White, Color.Black),
    };

    canvas.Flush();
    canvas.Flush();
    canvas.Flush();

    renderer.Received(1).RenderGlyph(14, 8, Arg.Any<Glyph>());
  }
}
