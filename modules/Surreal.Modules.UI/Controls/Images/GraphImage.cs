using System.Numerics;
using Surreal.Collections.Spans;
using Surreal.Graphics;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Grids;

namespace Surreal.UI.Controls.Images {
  public abstract class GraphImage : DynamicImage {
    public Color Color      { get; set; } = Color.White;
    public int   Resolution { get; set; } = 128;

    protected GraphImage(IGraphicsDevice device, Graphics.Textures.Image image)
        : base(device, image) {
    }

    protected override void Repaint(ImageRegion image) {
      var heights = new SpanList<float>(stackalloc float[Resolution]);

      EvaluateCurve(ref heights);

      var width = image.Width / heights.Capacity;

      for (var x = 1; x < width; x++) {
        var from = new Vector2(x - 1, heights[x - 1]);
        var to   = new Vector2(x, heights[x]);

        image.DrawLine(from, to, Color);
      }
    }

    protected abstract void EvaluateCurve(ref SpanList<float> points);
  }
}