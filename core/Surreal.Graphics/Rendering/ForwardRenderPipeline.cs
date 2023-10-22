using Surreal.Colors;
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
    private readonly RenderTarget _colorTarget = new(backend, new RenderTargetDescriptor
    {
      Format = TextureFormat.Rgba8,
      FilterMode = TextureFilterMode.Point,
      WrapMode = TextureWrapMode.ClampToEdge,
      DepthStencilFormat = DepthStencilFormat.None,
    });

    public override void OnBeginCamera(in RenderFrame frame, IRenderCamera camera)
    {
      base.OnBeginCamera(in frame, camera);

      var clearColor = camera.ClearColor.GetOrDefault(pipeline.ClearColor);

      _colorTarget.BindToDisplay();
      _colorTarget.ClearColorBuffer(clearColor);
    }

    public override void OnRenderCamera(in RenderFrame frame, IRenderCamera camera)
    {
      foreach (var visibleObject in camera.CullVisibleObjects())
      {
        visibleObject.Render(in frame);
      }
    }

    public override void OnEndCamera(in RenderFrame frame, IRenderCamera camera)
    {
      var viewportSize = frame.Viewport;

      // blit the color target to the back buffer
      _colorTarget.BlitToBackBuffer(
        sourceWidth: _colorTarget.Width,
        sourceHeight: _colorTarget.Height,
        destWidth: viewportSize.Width,
        destHeight: viewportSize.Height,
        mask: BlitMask.Color,
        filterMode: TextureFilterMode.Point
      );

      _colorTarget.UnbindFromDisplay();
    }

    public override void Dispose()
    {
      _colorTarget.Dispose();

      base.Dispose();
    }
  }
}
