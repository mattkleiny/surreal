namespace Surreal.Graphics.Experimental.Rendering {
  public interface IRenderingPass {
    RenderingStage Stage { get; }

    void Render(ref RenderingContext context);
  }
}