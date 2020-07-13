using Surreal.Framework.Parameters;
using Surreal.Graphics.Experimental.Rendering.PostProcessing;
using Surreal.Mathematics;

namespace Surreal.Graphics.Rendering.PostProcessing {
  public sealed class DistortionEffect : IPostProcessingEffect {
    public FloatParameter Amount     { get; } = new ClampedFloatParameter(0f, Range.Of(0f, 1f));
    public FloatParameter Resolution { get; } = new ClampedFloatParameter(0f, Range.Of(0f, 1f));

    public PostProcessingEffectStage Stage => PostProcessingEffectStage.AfterAll;

    public void Render(ref PostProcessingContext context) {
      var program = context.ShaderFactory.GetOrCreate("resx://Surreal/Resources/Shaders/PostProcessing/Distortion.shader");

      program.SetUniform("_Amount", Amount);
      program.SetUniform("_Resolution", Resolution);

      context.BlitImageEffect(program);
    }
  }
}