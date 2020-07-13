using Surreal.Collections;
using Surreal.Graphics;
using Surreal.Graphics.Textures;
using Surreal.UI.Controls.Images;

namespace Surreal.Diagnostics.Profiling.Controls {
  internal sealed class MemoryGraph : GraphImage {
    public MemoryGraph(IGraphicsDevice device)
        : base(device, new Pixmap(256, 256)) {
    }

    protected override void EvaluateCurve(ref SpanList<float> points) {
    }
  }
}