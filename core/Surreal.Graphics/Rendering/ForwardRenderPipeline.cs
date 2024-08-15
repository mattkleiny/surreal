using Surreal.Colors;
using Surreal.Diagnostics.Gizmos;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// A lightweight forward rendering pipeline.
/// </summary>
[RenderPipeline("Forward")]
public class ForwardRenderPipeline : MultiPassRenderPipeline
{
  public ForwardRenderPipeline(IGraphicsDevice device)
    : base(device)
  {
    Passes.Add(new DepthPass(device, this));
    Passes.Add(new ColorPass(device, this));
    Passes.Add(new GizmoPass(device, this));

    Contexts.Add(new SpriteContext(device));
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
  private sealed class DepthPass(IGraphicsDevice device, ForwardRenderPipeline pipeline) : RenderPass
  {
    private readonly RenderTarget _depthTarget = new(device, new RenderTargetDescriptor
    {
      Format = TextureFormat.R,
      FilterMode = TextureFilterMode.Linear,
      WrapMode = TextureWrapMode.ClampToEdge,
      DepthStencilFormat = DepthStencilFormat.Depth24
    });

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
  private sealed class ColorPass(IGraphicsDevice device, ForwardRenderPipeline pipeline) : RenderPass
  {
    private readonly Material _blitMaterial = new(device, ShaderProgram.LoadDefaultBlitShader(device));
    private readonly RenderTarget _colorTarget = new(device, new RenderTargetDescriptor
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

    public override void OnExecutePass(in RenderFrame frame, IRenderViewport viewport)
    {
      base.OnExecutePass(in frame, viewport);

      _colorTarget.BindToDisplay();

      foreach (var renderObject in viewport.CullVisibleObjects<IRenderObject>())
      {
        renderObject.Render(in frame);
      }
    }

    public override void OnEndFrame(in RenderFrame frame)
    {
      _colorTarget.UnbindFromDisplay();
      _colorTarget.BlitToFrameBuffer(_blitMaterial);

      base.OnEndFrame(in frame);
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
  private sealed class GizmoPass(IGraphicsDevice device, ForwardRenderPipeline pipeline) : RenderPass
  {
    private readonly Material _material = new(device, ShaderProgram.LoadDefaultWireShader(device));
    private readonly GeometryBatch _batch = new(device);

    public override bool IsEnabled => pipeline.EnableGizmos;

    public override void OnExecutePass(in RenderFrame frame, IRenderViewport viewport)
    {
      base.OnExecutePass(in frame, viewport);

      _material.Uniforms.Set("u_transform", viewport.ProjectionView);
      _batch.Material = _material;

      foreach (var gizmoObject in viewport.CullVisibleObjects<IGizmoObject>())
      {
        gizmoObject.RenderGizmos(_batch);
      }

      _batch.Flush();
    }

    public override void Dispose()
    {
      _batch.Dispose();

      base.Dispose();
    }
  }
}
