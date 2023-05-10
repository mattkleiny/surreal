namespace Surreal.Graphics.Rendering;

/// <summary>
/// A lightweight, render-pass based <see cref="RenderPipeline"/>.
/// </summary>
[RenderPipeline("Deferred")]
public sealed class DeferredRenderPipeline : MultiPassRenderPipeline
{
  public DeferredRenderPipeline(IGraphicsServer server)
  {
    Passes.Add(new ColorBufferPass(server));
    Passes.Add(new ShadowStencilPass(server));
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that collects color data.
  /// </summary>
  private sealed class ColorBufferPass : RenderPass
  {
    private readonly IGraphicsServer _server;

    public ColorBufferPass(IGraphicsServer server)
    {
      _server = server;
    }
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that collects stenciled shadow map data.
  /// </summary>
  private sealed class ShadowStencilPass : RenderPass
  {
    private readonly IGraphicsServer _server;

    public ShadowStencilPass(IGraphicsServer server)
    {
      _server = server;
    }
  }
}
