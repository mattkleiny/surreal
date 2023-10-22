using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// A lightweight forward rendering pipeline.
/// </summary>
[RenderPipeline("Forward")]
public sealed class ForwardRenderPipeline : MultiPassRenderPipeline
{
  public ForwardRenderPipeline(IGraphicsBackend backend)
    : base(backend)
  {
    Passes.Add(new ColorBufferPass(backend, this));
  }

  /// <summary>
  /// The color to clear the screen with.
  /// </summary>
  public Color ClearColor { get; set; } = Color.Clear;

  /// <summary>
  /// A <see cref="RenderPass"/> that collects color data.
  /// </summary>
  private sealed class ColorBufferPass(IGraphicsBackend backend, ForwardRenderPipeline pipeline) : RenderPass
  {
    /// <summary>
    /// The material used to blit the color target to the back buffer.
    /// </summary>
    private readonly Material _blitMaterial = new(backend, ShaderProgram.LoadDefaultBlitShader(backend));

    /// <summary>
    /// The main color target for the pass.
    /// </summary>
    private readonly RenderTarget _colorTarget = new(backend, new RenderTargetDescriptor
    {
      Format = TextureFormat.Rgba8,
      FilterMode = TextureFilterMode.Point,
      WrapMode = TextureWrapMode.ClampToEdge,
      DepthStencilFormat = DepthStencilFormat.None
    });

    public override void OnBeginFrame(in RenderFrame frame)
    {
      base.OnBeginFrame(in frame);

      _colorTarget.BindToDisplay();
      _colorTarget.ClearColorBuffer(pipeline.ClearColor);
    }

    public override void OnRenderViewport(in RenderFrame frame, IRenderViewport viewport)
    {
      foreach (var visibleObject in viewport.CullVisibleObjects())
      {
        visibleObject.Render(in frame);
      }
    }

    public override void OnEndFrame(in RenderFrame frame)
    {
      _colorTarget.UnbindFromDisplay();
      _colorTarget.BlitToBackBuffer(_blitMaterial, mask: BlitMask.Color);
    }

    public override void Dispose()
    {
      _colorTarget.Dispose();
      _blitMaterial.Dispose();

      base.Dispose();
    }
  }
}
