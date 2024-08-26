using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// An effect for a post-processing pipeline.
/// </summary>
public interface IRenderEffect : IDisposable
{
  void RenderEffect(in RenderFrame frame, RenderEffectContext context);
}

/// <summary>
/// The context for a <see cref="IRenderEffect"/>.
/// </summary>
public sealed class RenderEffectContext(RenderTarget source, RenderTarget target)
{
  public RenderTarget FrontBuffer { get; private set; } = source;
  public RenderTarget BackBuffer { get; private set; } = target;

  /// <summary>
  /// Blits the given material to the backbuffer.
  /// </summary>
  public void BlitMaterial(Material material)
  {
    FrontBuffer.BlitToRenderTarget(BackBuffer, material, ShaderProperty.Texture);
  }

  /// <summary>
  /// Resets the buffers.
  /// </summary>
  public void Reset()
  {
    FrontBuffer = source;
    BackBuffer = target;
  }

  /// <summary>
  /// Flips the buffers.
  /// </summary>
  public void FlipBuffers()
  {
    (FrontBuffer, BackBuffer) = (BackBuffer, FrontBuffer);
  }
}

/// <summary>
/// A post-processing effect that applies chromatic aberration.
/// </summary>
public sealed class ChromaticAberrationEffect(IGraphicsDevice device) : IRenderEffect
{
  private readonly Material _material = new(device, ShaderProgram.Load(device, "resx://Surreal.Graphics/Assets/Embedded/shaders/effects/effect-aberration.glsl"));

  /// <summary>
  /// The intensity of the effect.
  /// </summary>
  public float Intensity { get; set; } = 0.1f;

  public void RenderEffect(in RenderFrame frame, RenderEffectContext context)
  {
    _material.Uniforms.Set("u_intensity", Intensity);

    context.BlitMaterial(_material);
  }

  public void Dispose()
  {
    _material.Dispose();
  }
}
