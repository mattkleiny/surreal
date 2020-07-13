namespace Surreal.Graphics.Experimental.Rendering.Passes {
  public sealed class OpaquePass : IRenderingPass {
    RenderingStage IRenderingPass.Stage => RenderingStage.BeforeOpaque;

    public Color BackgroundColor { get; set; } = Color.Black;
    public bool  ClearColor      { get; set; } = true;
    public bool  ClearDepth      { get; set; } = true;

    public void Render(ref RenderingContext context) {
      context.Commands.ClearRenderTarget(BackgroundColor, ClearColor, ClearDepth);
    }
  }
}