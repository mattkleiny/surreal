using Surreal.Colors;
using Surreal.Diagnostics.Gizmos;
using Surreal.Graphics.Canvases;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Sprites;
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
    Passes.Add(new DepthPass(backend, this));
    Passes.Add(new ColorPass(backend, this));
    Passes.Add(new GizmoPass(backend, this));

    Contexts.Add(new CanvasContext(backend));
  }

  /// <summary>
  /// Determines if a depth pass is required
  /// </summary>
  public bool RequireDepthPass { get; set; } = true;

  /// <summary>
  /// Determines if gizmos are enabled.
  /// </summary>
  public bool EnableGizmos { get; set; } = true;

  /// <summary>
  /// The color to clear the screen with.
  /// </summary>
  public Color ClearColor { get; set; } = Color.Black;

  /// <summary>
  /// A <see cref="RenderPass"/> that collects depth data.
  /// </summary>
  private sealed class DepthPass(IGraphicsBackend backend, ForwardRenderPipeline pipeline) : RenderPass
  {
    /// <summary>
    /// The main color target for the pass.
    /// </summary>
    private readonly RenderTarget _depthTarget = new(backend, new RenderTargetDescriptor
    {
      Format = TextureFormat.R,
      FilterMode = TextureFilterMode.Linear,
      WrapMode = TextureWrapMode.ClampToEdge,
      DepthStencilFormat = DepthStencilFormat.Depth24
    });

    /// <inheritdoc/>
    public override bool IsEnabled => pipeline.RequireDepthPass;

    public override void OnExecutePass(in RenderFrame frame, IRenderViewport viewport)
    {
      base.OnExecutePass(in frame, viewport);

      _depthTarget.ResizeFrameBuffer(frame.Viewport.Width, frame.Viewport.Height);
      _depthTarget.BindToDisplay();

      _depthTarget.ClearColorBuffer(pipeline.ClearColor);
      _depthTarget.ClearDepthBuffer(1f);

      foreach (var renderObject in viewport.CullVisibleObjects<IRenderObject>())
      {
        renderObject.Render(in frame);
      }

      _depthTarget.UnbindFromDisplay();
    }

    public override void Dispose()
    {
      _depthTarget.Dispose();

      base.Dispose();
    }
  }

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

    public override void OnExecutePass(in RenderFrame frame, IRenderViewport viewport)
    {
      base.OnExecutePass(in frame, viewport);

      _colorTarget.ResizeFrameBuffer(frame.Viewport.Width, frame.Viewport.Height);
      _colorTarget.BindToDisplay();

      _colorTarget.ClearColorBuffer(pipeline.ClearColor);

      foreach (var renderObject in viewport.CullVisibleObjects<IRenderObject>())
      {
        renderObject.Render(in frame);
      }

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
    private readonly GeometryBatch _geometryBatch = new(backend);

    /// <inheritdoc/>
    public override bool IsEnabled => pipeline.EnableGizmos;

    public override void OnExecutePass(in RenderFrame frame, IRenderViewport viewport)
    {
      base.OnExecutePass(in frame, viewport);

      _gizmoMaterial.Properties.SetProperty("u_transform", viewport.ProjectionView);

      _geometryBatch.Material = _gizmoMaterial;

      if (viewport is IGizmoObject viewportGizmos)
      {
        viewportGizmos.RenderGizmos(_geometryBatch);
      }

      foreach (var gizmoObject in viewport.CullVisibleObjects<IGizmoObject>())
      {
        gizmoObject.RenderGizmos(_geometryBatch);
      }

      _geometryBatch.Flush();
    }

    public override void Dispose()
    {
      _geometryBatch.Dispose();

      base.Dispose();
    }
  }
}
