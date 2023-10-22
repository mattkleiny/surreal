using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// A lightweight forward rendering pipeline.
/// </summary>
[RenderPipeline("Forward")]
public sealed class ForwardRenderPipeline : MultiPassRenderPipeline
{
  public ForwardRenderPipeline(IGraphicsBackend backend) : base(backend)
  {
    Passes.Add(new ColorBufferPass(backend));
    Passes.Add(new ShadowStencilPass(backend));
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that collects color data.
  /// </summary>
  private sealed class ColorBufferPass(IGraphicsBackend backend) : RenderPass
  {
    private readonly RenderTarget _colorTarget = new(backend, new RenderTargetDescriptor
    {
      Format = TextureFormat.Rgba8,
      FilterMode = TextureFilterMode.Point,
      WrapMode = TextureWrapMode.ClampToEdge,
      DepthBits = DepthBits.None,
      Stencil = StencilBits.None
    });

    public override void OnBeginFrame(in RenderFrame frame)
    {
      // TODO: clear the color target
    }

    public override void OnExecutePass(in RenderFrame frame)
    {
      foreach (var renderObject in frame.VisibleObjects)
      {
        renderObject.Render(in frame);
      }
    }

    public override void OnEndFrame(in RenderFrame frame)
    {
      // TODO: blit the color target to the back buffer
    }

    public override void Dispose()
    {
      _colorTarget.Dispose();

      base.Dispose();
    }
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that collects stenciled shadow map data.
  /// </summary>
  private sealed class ShadowStencilPass(IGraphicsBackend backend) : RenderPass
  {
    private readonly RenderTarget _shadowStencilTarget = new(backend, new RenderTargetDescriptor
    {
      Format = TextureFormat.R8,
      FilterMode = TextureFilterMode.Point,
      WrapMode = TextureWrapMode.ClampToEdge,
      DepthBits = DepthBits.None,
      Stencil = StencilBits.Eight
    });

    public override void OnBeginFrame(in RenderFrame frame)
    {
      // TODO: clear the stencil target
    }

    public override void Dispose()
    {
      _shadowStencilTarget.Dispose();

      base.Dispose();
    }
  }
}
