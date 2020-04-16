using System.Numerics;
using Surreal.Collections;
using Surreal.Graphics;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Grids;

namespace Surreal.UI.Controls.Images
{
  public abstract class GraphImage : RawImage
  {
    public Color Color      { get; set; } = Color.White;
    public int   Resolution { get; set; } = 128;

    protected GraphImage(IGraphicsDevice device, Pixmap pixmap)
      : base(device, pixmap)
    {
    }

    protected override void Repaint(PixmapRegion pixmap)
    {
      var heights = new SpanList<float>(stackalloc float[Resolution]);

      EvaluateCurve(ref heights);

      var width = pixmap.Width / heights.Count;

      for (var x = 1; x < width; x++)
      {
        var from = new Vector2(x - 1, heights[x - 1]);
        var to   = new Vector2(x, heights[x]);

        pixmap.DrawLine(from, to, Color);
      }
    }

    protected abstract void EvaluateCurve(ref SpanList<float> points);
  }
}