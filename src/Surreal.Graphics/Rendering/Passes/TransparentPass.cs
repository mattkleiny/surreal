namespace Surreal.Graphics.Rendering.Passes {
  public sealed class TransparentPass : IRenderingPass {
    RenderingStage IRenderingPass.Stage => RenderingStage.BeforeTransparent;

    public void Render(ref RenderingContext context) {
    }
  }
}