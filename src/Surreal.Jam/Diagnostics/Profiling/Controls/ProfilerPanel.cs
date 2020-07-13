using Surreal.Graphics;
using Surreal.Graphics.Fonts;
using Surreal.UI.Controls;

namespace Surreal.Diagnostics.Profiling.Controls {
  internal sealed class ProfilerPanel : Panel {
    public ProfilerPanel(IGraphicsDevice device, BitmapFont font, InMemoryProfilerSampler sampler) {
      Add(new Panel {
          new Label(font, "Profiler"),
          new MemoryGraph(device),
          new SamplerGraph(device, sampler)
      });
    }
  }
}