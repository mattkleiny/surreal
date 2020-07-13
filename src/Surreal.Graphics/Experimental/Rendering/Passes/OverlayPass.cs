namespace Surreal.Graphics.Experimental.Rendering.Passes {
  public sealed class OverlayPass : IRenderingPass {
    RenderingStage IRenderingPass.Stage => RenderingStage.AfterTransparent;

    public void Render(ref RenderingContext context) {
    }
  }
}