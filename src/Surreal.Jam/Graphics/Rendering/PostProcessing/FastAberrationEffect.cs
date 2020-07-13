using System.Numerics;
using Surreal.Framework.Parameters;
using Surreal.Mathematics;

namespace Surreal.Graphics.Rendering.PostProcessing {
  public sealed class FastAberrationEffect : IPostProcessingEffect {
    public Vector2Parameter Direction { get; } = new Vector2Parameter(new Vector2(1f, 1f));
    public FloatParameter   Amount    { get; } = new ClampedFloatParameter(0f, Range.Of(0f, 1f));

    public PostProcessingEffectStage Stage => PostProcessingEffectStage.StandardEffects;

    public void Render(ref PostProcessingContext context) {
      var program = context.ShaderFactory.GetOrCreate("resx://Surreal/Resources/Shaders/PostProcessing/FastAberration.shader");

      program.SetUniform("_Direction", Direction);
      program.SetUniform("_Amount", Amount);

      context.BlitImageEffect(program);
    }
  }
}