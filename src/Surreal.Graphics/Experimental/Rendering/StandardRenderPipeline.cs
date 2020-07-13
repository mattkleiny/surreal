using System;
using Surreal.Graphics.Cameras;

namespace Surreal.Graphics.Experimental.Rendering {
  public class StandardRenderPipeline : RenderingPipeline {
    private readonly IRenderer renderer;

    public StandardRenderPipeline(IRenderer renderer) {
      this.renderer = renderer;
    }

    protected override void RenderCamera(ICamera camera) {
      base.RenderCamera(camera);

      renderer.RenderCamera(camera);
    }

    public override void Dispose() {
      if (renderer is IDisposable disposable) {
        disposable.Dispose();
      }

      base.Dispose();
    }
  }
}