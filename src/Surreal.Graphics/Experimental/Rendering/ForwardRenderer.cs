using Surreal.Assets;
using Surreal.Graphics.Experimental.Rendering.Culling;
using Surreal.Graphics.Experimental.Rendering.Passes;

namespace Surreal.Graphics.Experimental.Rendering {
  public class ForwardRenderer : Renderer {
    public OpaquePass         OpaquePass         { get; } = new OpaquePass();
    public TransparentPass    TransparentPass    { get; } = new TransparentPass();
    public OverlayPass        OverlayPass        { get; } = new OverlayPass();
    public PostProcessingPass PostProcessingPass { get; }

    public ForwardRenderer(
        IGraphicsDevice device,
        ICullingStrategy cullingStrategy,
        in FrameBufferDescriptor colorDescriptor,
        in FrameBufferDescriptor depthDescriptor,
        IAssetResolver assets
    )
        : base(device, cullingStrategy, colorDescriptor, depthDescriptor) {
      PostProcessingPass = new PostProcessingPass(device, assets, colorDescriptor);

      Add(OpaquePass);
      Add(TransparentPass);
      Add(OverlayPass);
      Add(PostProcessingPass);
    }
  }
}