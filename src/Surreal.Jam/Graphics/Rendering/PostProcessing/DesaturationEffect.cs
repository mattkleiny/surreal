using Surreal.Framework.Parameters;
using Surreal.Mathematics;

namespace Surreal.Graphics.Rendering.PostProcessing {
  public sealed class DesaturationEffect : IPostProcessingEffect {
    public FloatParameter Amount { get; } = new ClampedFloatParameter(0f, Range.Of(0f, 1f));

    public PostProcessingEffectStage Stage => PostProcessingEffectStage.AfterAll;

    public void Render(ref PostProcessingContext context) {
      var program = context.ShaderFactory.GetOrCreate("resx://Surreal/Resources/Shaders/PostProcessing/Desaturation.shader");

      program.SetUniform("_Amount", Amount);

      context.BlitImageEffect(program);
    }
  }
}