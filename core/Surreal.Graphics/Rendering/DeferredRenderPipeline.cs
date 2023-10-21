namespace Surreal.Graphics.Rendering;

/// <summary>
/// A lightweight, render-pass based <see cref="RenderPipeline"/>.
/// </summary>
[RenderPipeline("Deferred")]
public sealed class DeferredRenderPipeline : MultiPassRenderPipeline
{
  public DeferredRenderPipeline(IGraphicsBackend backend)
  {
    Passes.Add(new ColorBufferPass());
    Passes.Add(new ShadowStencilPass());
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that collects color data.
  /// </summary>
  private sealed class ColorBufferPass : RenderPass;

  /// <summary>
  /// A <see cref="RenderPass"/> that collects stenciled shadow map data.
  /// </summary>
  private sealed class ShadowStencilPass : RenderPass;
}
