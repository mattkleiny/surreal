using System.Linq;
using Surreal.Collections;
using Surreal.Graphics;
using Surreal.UI.Controls.Images;
using Image = Surreal.Graphics.Textures.Image;

namespace Surreal.Diagnostics.Profiling.Controls {
  internal sealed class SamplerGraph : GraphImage {
    private readonly InMemoryProfilerSampler sampler;

    public SamplerGraph(IGraphicsDevice device, InMemoryProfilerSampler sampler)
        : base(device, new Image(256, 256)) {
      this.sampler = sampler;
    }

    protected override void EvaluateCurve(ref SpanList<float> points) {
      foreach (var sample in sampler.Samplers.First().Take(points.Capacity)) {
        points.Add(sample.Milliseconds);
      }
    }
  }
}