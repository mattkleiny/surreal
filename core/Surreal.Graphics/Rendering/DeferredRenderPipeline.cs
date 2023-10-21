namespace Surreal.Graphics.Rendering;

/// <summary>
/// A lightweight, render-pass based <see cref="RenderPipeline"/>.
/// </summary>
[RenderPipeline("Deferred")]
public sealed class DeferredRenderPipeline : MultiPassRenderPipeline
{
  public DeferredRenderPipeline(GraphicsContext context)
  {
    Passes.Add(new ColorBufferPass(context));
    Passes.Add(new ShadowStencilPass(context));
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that collects color data.
  /// </summary>
  private sealed class ColorBufferPass(GraphicsContext context) : RenderPass
  {
    private readonly GraphicsContext _context = context;
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that collects stenciled shadow map data.
  /// </summary>
  private sealed class ShadowStencilPass(GraphicsContext context) : RenderPass
  {
    private readonly GraphicsContext _context = context;
  }
}
