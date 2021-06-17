using Surreal.Collections;
using Surreal.Graphics;
using Surreal.UI.Controls.Images;
using Image = Surreal.Graphics.Textures.Image;

namespace Surreal.Diagnostics.Profiling.Controls {
  internal sealed class MemoryGraph : GraphImage {
    public MemoryGraph(IGraphicsDevice device)
        : base(device, new Image(256, 256)) {
    }

    protected override void EvaluateCurve(ref SpanList<float> points) {
    }
  }
}