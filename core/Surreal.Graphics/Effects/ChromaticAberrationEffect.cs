using Surreal.Graphics.Materials;
using Surreal.Graphics.Rendering;

namespace Surreal.Graphics.Effects;

/// <summary>
/// A post-processing effect that applies chromatic aberration.
/// </summary>
public sealed class ChromaticAberrationEffect(IGraphicsDevice device) : IPostProcessingEffect
{
  private readonly Material _material = new(device, ShaderProgram.LoadDefaultBlitShader(device));

  /// <summary>
  /// The intensity of the effect.
  /// </summary>
  public float Intensity { get; set; } = 0.1f;

  public void RenderEffect(in RenderFrame frame, PostProcessingContext context)
  {
    _material.Uniforms.Set("u_intensity", Intensity);

    context.BlitMaterial(_material);
  }

  public void Dispose()
  {
    _material.Dispose();
  }
}
