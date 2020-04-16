namespace Surreal.Graphics.Rendering
{
  public interface IRenderingPass
  {
    RenderingStage Stage { get; }

    void Render(ref RenderingContext context);
  }
}