using Surreal.Framework.Parameters;
using Surreal.Graphics.Rendering;
using Surreal.Mathematics;

namespace Avventura.Graphics.Passes {
  public sealed class LightingPass : IRenderingPass {
    public FloatParameter GlobalIntensity { get; } = new ClampedFloatParameter(0f, Range.Of(0f, 1f));

    RenderingStage IRenderingPass.Stage => RenderingStage.BeforeOpaque;

    public void Render(ref RenderingContext context) {
    }
  }
}