using Surreal.Colors;
using Surreal.Graphics.Gizmos;
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
    Passes.Add(new ColorPass(backend, this));
    Passes.Add(new GizmoPass(backend, this));
  }

  /// <summary>
  /// Determines if gizmos are enabled.
  /// </summary>
  public bool EnableGizmos { get; set; } = Debugger.IsAttached;

  /// <summary>
  /// The color to clear the screen with.
  /// </summary>
  public Color ClearColor { get; set; } = Color.Clear;

  /// <summary>
  /// A <see cref="RenderPass"/> that collects color data.
  /// </summary>
  private sealed class ColorPass(IGraphicsBackend backend, ForwardRenderPipeline pipeline) : RenderPass
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

      _colorTarget.ResizeFrameBuffer(frame.Viewport.Width, frame.Viewport.Height);
      _colorTarget.BindToDisplay();
      _colorTarget.ClearColorBuffer(pipeline.ClearColor);
    }

    public override void OnRenderViewport(in RenderFrame frame, IRenderViewport viewport)
    {
      foreach (var renderObject in viewport.CullVisibleObjects<IRenderObject>())
      {
        renderObject.Render(in frame);
      }
    }

    public override void OnEndFrame(in RenderFrame frame)
    {
      _colorTarget.UnbindFromDisplay();
      _colorTarget.BlitToBackBuffer(_blitMaterial);
    }

    public override void Dispose()
    {
      _colorTarget.Dispose();
      _blitMaterial.Dispose();

      base.Dispose();
    }
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that renders gizmos.
  /// </summary>
  private sealed class GizmoPass(IGraphicsBackend backend, ForwardRenderPipeline pipeline) : RenderPass
  {
    /// <summary>
    /// The  material used to render gizmos.
    /// </summary>
    private readonly Material _gizmoMaterial = new(backend, ShaderProgram.LoadDefaultWireShader(backend));

    /// <summary>
    /// The batch used to render gizmos.
    /// </summary>
    public GizmoBatch GizmoBatch { get; } = new(backend);

    /// <inheritdoc/>
    public override bool IsEnabled => pipeline.EnableGizmos;

    public override void OnEndViewport(in RenderFrame frame, IRenderViewport viewport)
    {
      _gizmoMaterial.Properties.SetProperty("u_projectionView", viewport.ProjectionView);

      GizmoBatch.Begin(_gizmoMaterial);

      foreach (var gizmoObject in viewport.CullVisibleObjects<IGizmoObject>())
      {
        gizmoObject.RenderGizmos(in frame, GizmoBatch);
      }

      GizmoBatch.Flush();

      base.OnEndViewport(in frame, viewport);
    }

    public override void Dispose()
    {
      GizmoBatch.Dispose();

      base.Dispose();
    }
  }
}
