namespace Surreal.Graphics.Rendering.PostProcessing {
  public interface IPostProcessingEffect {
    PostProcessingEffectStage Stage { get; }

    void Render(ref PostProcessingContext context);
  }
}