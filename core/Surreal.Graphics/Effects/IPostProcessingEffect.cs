using Surreal.Graphics.Materials;
using Surreal.Graphics.Rendering;

namespace Surreal.Graphics.Effects;

/// <summary>
/// A post-processing effect.
/// </summary>
public interface IPostProcessingEffect : IDisposable
{
  /// <summary>
  /// Renders the effect.
  /// </summary>
  void RenderEffect(in RenderFrame frame, PostProcessingContext context);
}

/// <summary>
/// The context for post-processing effects.
/// </summary>
public sealed class PostProcessingContext(RenderTarget source, RenderTarget target)
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
